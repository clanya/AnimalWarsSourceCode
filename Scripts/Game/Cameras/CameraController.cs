using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using VContainer;
using Game.Stages.Managers;

public class CameraController : MonoBehaviour
{
    [Inject] MapManager mapManager;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float cameraHeight = 5f;
    [SerializeField] private float cameraAngle = 50f;
    [SerializeField] private float horizontalDistance = 4.5f;
    [SerializeField] private float sensitivity = 0.01f;

    private bool pushedMouseRightButton => Input.GetMouseButtonDown(1);
    private bool pushingMouseRightButton => Input.GetMouseButton(1);

    private bool IsFollowing;

    private float limitX;
    private float limitZ;

    private void Start()
    {
        targetCamera.transform.rotation = Quaternion.Euler(new Vector3(cameraAngle, 0, 0));

        limitX = mapManager.limitX;
        limitZ = mapManager.limitY-horizontalDistance;

        MouseControlObservables();
    }

    //右クリックしたままドラッグしたときにカメラを動かす
    private void MouseControlObservables()
    {
        this.ObserveEveryValueChanged(x => x.pushedMouseRightButton)
            .Where(x => x)
            .Merge()
            .Subscribe(_ =>
            {
                var clickedPoint = Input.mousePosition;
                var firstPosition = targetCamera.transform.position;

                this.UpdateAsObservable()
                    .TakeWhile(_ => pushingMouseRightButton)
                    .Subscribe(_ =>
                    {
                        var vec = (clickedPoint - Input.mousePosition) * sensitivity;
                        var point = firstPosition + new Vector3(vec.x, 0, vec.y);

                        //カメラの移動範囲を制限
                        point.x = Mathf.Clamp(point.x, 0, limitX);
                        point.z = Mathf.Clamp(point.z, -horizontalDistance, limitZ);

                        targetCamera.transform.position = new Vector3(point.x, cameraHeight, point.z);
                    })
                    .AddTo(this);
            })
            .AddTo(this);
    }

    //指定の位置の真上に動く
    private void Look(Vector3 targetPos)
    {
        targetCamera.transform.position = new Vector3(targetPos.x, cameraHeight, targetPos.z - horizontalDistance);
    }

    /// <summary>
    /// 対象の位置をフォーカスするように動く
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="token"></param>
    /// <param name="time"></param>
    public async UniTask Forcus(Vector3 targetPos, CancellationToken token, float time=0.5f)
    {
        var firstPostion = targetCamera.transform.position;
        firstPostion.z += horizontalDistance;
        var elapsedTime = 0f;

        while (true)
        {
            var timeRate = elapsedTime / time;

            var x = Mathf.Lerp(firstPostion.x, targetPos.x, timeRate);
            var z = Mathf.Lerp(firstPostion.z, targetPos.z, timeRate);

            Look(new Vector3(x, 0, z));

            if (elapsedTime >= time) break;

            await UniTask.DelayFrame(1, cancellationToken: token);

            elapsedTime += Time.deltaTime;
        }
    }

    public async void StartFollow(Transform target, CancellationToken token)
    {
        IsFollowing = true;

        while (IsFollowing)
        {
            //var pos = new Vector3(target.position.x, cameraHeight, target.position.z);
            //targetCamera.transform.position = pos;
            Look(target.transform.position);

            await UniTask.DelayFrame(1, cancellationToken: token);
        }

        Debug.Log("Stop follow");
    }

    public void StopFollow()
    {
        IsFollowing = false;
    }
}
