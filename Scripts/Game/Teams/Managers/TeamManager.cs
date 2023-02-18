using Game.Character;
using System.Collections.Generic;
using System.Linq;

namespace Game.Teams.Managers
{
    public class TeamManager
    {
        private Dictionary<int, CharacterType> teamDic = new Dictionary<int, CharacterType>();

        public IEnumerable<int> teamMemberNumbers => teamDic.Where(x => x.Value != CharacterType.None).Select(x => x.Key);
        public IEnumerable<CharacterType> teamCharacterTypes => teamDic.Values.Where(x => x != CharacterType.None);

        public bool IsExistTeamMenber => teamDic.Count > 0 && !teamDic.Values.All(x => x == CharacterType.None);

        public void SetTeamMember(int number, CharacterType characterType)
        {
            if (teamDic.ContainsKey(number))
            {
                teamDic[number] = characterType;
            }
            else
            {
                teamDic.Add(number, characterType);
            }
        }

        public void ClearTeam()
        {
            teamDic = teamDic.ToDictionary(x => x.Key, _ => CharacterType.None);
        }
    }
}

