using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System;

namespace MangaUpdates_Notifications.Extensions
{
    // https://stackoverflow.com/a/31040692
    public static class DirectoryExtensions
    {
        public static bool IsWriteable(this DirectoryInfo directory, AuthorizationRuleCollection? rules = null, WindowsIdentity? identity = null)
        {
            try
            {
                rules ??= directory.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
                identity ??= WindowsIdentity.GetCurrent();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            bool isAllow = false;
            string userSid = identity.User.Value;

            foreach (FileSystemAccessRule rule in rules)
            {
                if (rule.IdentityReference.ToString() == userSid || identity.Groups.Contains(rule.IdentityReference))
                {
                    if ((rule.FileSystemRights.HasFlag(FileSystemRights.Write) ||
                         rule.FileSystemRights.HasFlag(FileSystemRights.WriteAttributes) ||
                         rule.FileSystemRights.HasFlag(FileSystemRights.WriteData) ||
                         rule.FileSystemRights.HasFlag(FileSystemRights.CreateDirectories) ||
                         rule.FileSystemRights.HasFlag(FileSystemRights.CreateFiles)) && rule.AccessControlType == AccessControlType.Deny)
                        return false;
                    if (rule.FileSystemRights.HasFlag(FileSystemRights.Write) &&
                         rule.FileSystemRights.HasFlag(FileSystemRights.WriteAttributes) &&
                         rule.FileSystemRights.HasFlag(FileSystemRights.WriteData) &&
                         rule.FileSystemRights.HasFlag(FileSystemRights.CreateDirectories) &&
                         rule.FileSystemRights.HasFlag(FileSystemRights.CreateFiles) && rule.AccessControlType == AccessControlType.Allow)
                        isAllow = true;

                }
            }
            return isAllow;
        }

        public static bool IsReadable(this DirectoryInfo directory, AuthorizationRuleCollection? rules = null, WindowsIdentity? identity = null)
        {
            try
            {
                rules ??= directory.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
                identity ??= WindowsIdentity.GetCurrent();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            bool isAllow = false;
            string userSid = identity.User.Value;

            foreach (FileSystemAccessRule rule in rules)
            {
                if (rule.IdentityReference.ToString() == userSid || identity.Groups.Contains(rule.IdentityReference))
                {
                    if ((rule.FileSystemRights.HasFlag(FileSystemRights.Read) ||
                         rule.FileSystemRights.HasFlag(FileSystemRights.ReadAttributes) ||
                         rule.FileSystemRights.HasFlag(FileSystemRights.ReadData)) && rule.AccessControlType == AccessControlType.Deny)
                        return false;
                    if (rule.FileSystemRights.HasFlag(FileSystemRights.Read) &&
                         rule.FileSystemRights.HasFlag(FileSystemRights.ReadAttributes) &&
                         rule.FileSystemRights.HasFlag(FileSystemRights.ReadData) && rule.AccessControlType == AccessControlType.Allow)
                        isAllow = true;

                }
            }
            return isAllow;
        }

        public static bool IsAccessible(this DirectoryInfo directory)
        {
            AuthorizationRuleCollection rules;
            WindowsIdentity identity;
            try
            {
                rules = directory.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
                identity = WindowsIdentity.GetCurrent();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return directory.IsReadable(rules, identity) && directory.IsWriteable(rules, identity);
        }
    }
}