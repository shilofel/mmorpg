using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Models;
using SkillBridge.Message;
using Common.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
namespace Managers
{
    class FriendManager:Singleton<FriendManager>
    {
        public List<NFriendInfo> allFriends;
        private List<NFriendInfo> filteredFriends;

        public void Init(List<NFriendInfo> friends)
        {
            this.allFriends = friends;
            this.filteredFriends = new List<NFriendInfo>(friends);
        }

        public List<NFriendInfo> GetFilteredAndSortedFriends(string searchKeyword = "")
        {
            List<NFriendInfo> result;

            if (string.IsNullOrEmpty(searchKeyword))
            {
                result = new List<NFriendInfo>(allFriends);
            }
            else
            {
                string keyword = searchKeyword.ToLower();
                result = allFriends.Where(f => 
                    f.friendInfo.Name.ToLower().Contains(keyword) ||
                    f.friendInfo.Id.ToString().Contains(keyword)
                ).ToList();
            }

            return SortFriends(result);
        }

        private List<NFriendInfo> SortFriends(List<NFriendInfo> friends)
        {
            return friends.OrderByDescending(f => f.Status)
                         .ThenBy(f => f.friendInfo.Name)
                         .ToList();
        }

        public void RefreshFilteredFriends()
        {
            this.filteredFriends = new List<NFriendInfo>(allFriends);
        }
    }
}