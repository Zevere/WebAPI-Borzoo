using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public static class Extensions
    {
        public static void CopyTo(this User _this, User other)
        {
            other.Id = _this.Id;
            other.DisplayId = _this.DisplayId;
            other.PassphraseHash = _this.PassphraseHash;
            other.FirstName = _this.FirstName;
            other.JoinedAt = _this.JoinedAt.ToUniversalTime();
            other.LastName = _this.LastName;
            other.IsDeleted = _this.IsDeleted;
            other.Token = _this.Token;
            other.ModifiedAt = _this.ModifiedAt?.ToUniversalTime();
        }
    }
}