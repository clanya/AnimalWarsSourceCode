using Data.DataTable;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;

namespace Game.Stages
{
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField] private LandscapePrefabTable landscapePrefabTable;
        [SerializeField] private Transform landscapeParent;

        //配列すべてを見る
        public void GenerateLandscape(ReadOnlyCollection<ReadOnlyCollection<LandscapeType>> data)
        {
            int maxX = data.Count;
            int maxZ = data[0].Count;

            for(int x=0;x<maxX; x++)
            {
                for(int z = 0; z < maxZ; z++)
                {
                    if (data[x][z] == LandscapeType.None)
                        continue;

                    var generatePos = new Vector3(x, 0, z);
                    GenerateLandscape(data[x][z],generatePos);
                }
            }
        }

        //実際にオブジェクトを生成する
        private void GenerateLandscape(LandscapeType landscapeType, Vector3 generatePos)
        {
            var prefab = landscapePrefabTable.LandscapeDatTable.FirstOrDefault(x => x.Landscape == landscapeType).LandscapePrefab;
            var obj = Instantiate(prefab, generatePos, Quaternion.identity);
            obj.transform.SetParent(landscapeParent);
        }
    }
}

