using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatAPI_New.Models
{
    public class LoginAuth
    {
        public string AuthKey { get; set; }
        public string TwoFactorKey { get; set; }
    }

    public class AccountDeletionLog
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("deletionScheduled")]
        public DateTime? DeletionScheduled { get; set; }

        [JsonProperty("dateTime")]
        public DateTime? DateTime { get; set; }
    }

    public class PastDisplayName
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class VRChatUserInfo
    {
        [JsonProperty("acceptedTOSVersion")]
        public int AcceptedTOSVersion { get; set; }

        [JsonProperty("accountDeletionDate")]
        public string AccountDeletionDate { get; set; }

        [JsonProperty("accountDeletionLog")]
        public List<AccountDeletionLog> AccountDeletionLog { get; set; }

        [JsonProperty("activeFriends")]
        public List<string> ActiveFriends { get; set; }

        [JsonProperty("allowAvatarCopying")]
        public bool AllowAvatarCopying { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("bioLinks")]
        public List<string> BioLinks { get; set; }

        [JsonProperty("currentAvatar")]
        public string CurrentAvatar { get; set; }

        [JsonProperty("currentAvatarAssetUrl")]
        public string CurrentAvatarAssetUrl { get; set; }

        [JsonProperty("currentAvatarImageUrl")]
        public string CurrentAvatarImageUrl { get; set; }

        [JsonProperty("currentAvatarThumbnailImageUrl")]
        public string CurrentAvatarThumbnailImageUrl { get; set; }

        [JsonProperty("date_joined")]
        public string Date_joined { get; set; }

        [JsonProperty("developerType")]
        public string DeveloperType { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("fallbackAvatar")]
        public string FallbackAvatar { get; set; }

        [JsonProperty("friendKey")]
        public string FriendKey { get; set; }

        [JsonProperty("friends")]
        public List<string> Friends { get; set; }

        [JsonProperty("hasBirthday")]
        public bool HasBirthday { get; set; }

        [JsonProperty("hasEmail")]
        public bool HasEmail { get; set; }

        [JsonProperty("hasLoggedInFromClient")]
        public bool HasLoggedInFromClient { get; set; }

        [JsonProperty("hasPendingEmail")]
        public bool HasPendingEmail { get; set; }

        [JsonProperty("homeLocation")]
        public string HomeLocation { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isFriend")]
        public bool IsFriend { get; set; }

        [JsonProperty("last_login")]
        public DateTime? LastLogin { get; set; }

        [JsonProperty("last_platform")]
        public string LastPlatform { get; set; }

        [JsonProperty("obfuscatedEmail")]
        public string ObfuscatedEmail { get; set; }

        [JsonProperty("obfuscatedPendingEmail")]
        public string ObfuscatedPendingEmail { get; set; }

        [JsonProperty("oculusId")]
        public string OculusId { get; set; }

        [JsonProperty("offlineFriends")]
        public List<string> OfflineFriends { get; set; }

        [JsonProperty("onlineFriends")]
        public List<string> OnlineFriends { get; set; }

        [JsonProperty("pastDisplayNames")]
        public List<PastDisplayName> PastDisplayNames { get; set; }

        [JsonProperty("profilePicOverride")]
        public string ProfilePicOverride { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { get; set; }

        [JsonProperty("statusFirstTime")]
        public bool StatusFirstTime { get; set; }

        [JsonProperty("statusHistory")]
        public List<string> StatusHistory { get; set; }

        [JsonProperty("steamDetails")]
        public SteamDetails SteamDetails { get; set; }

        [JsonProperty("steamId")]
        public string SteamId { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("twoFactorAuthEnabled")]
        public bool TwoFactorAuthEnabled { get; set; }

        [JsonProperty("twoFactorAuthEnabledDate")]
        public DateTime? TwoFactorAuthEnabledDate { get; set; }

        [JsonProperty("unsubscribe")]
        public bool Unsubscribe { get; set; }

        [JsonProperty("userIcon")]
        public string UserIcon { get; set; }

        [JsonIgnore]
        public LoginAuth Details { get; set; }
    }

    public class SteamDetails
    {
    }

}
