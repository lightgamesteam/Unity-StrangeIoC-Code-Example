
namespace PFS.Assets.Scripts.Models.Requests
{
    public class ChildEditRequestModel : BasicRequestModel
    {
        public string id;
        public string nickname;
        public string surname;
        public string avatar;
        public string email;

        public void InitData(ChildModel child)
        {
            id = child.Id;
            nickname = child.Nickname;
            surname = child.Surname;
            email = child.Email;
            avatar = child.AvatarId;
        }
    }
}

