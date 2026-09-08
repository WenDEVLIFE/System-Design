using System;

namespace System_Design
{
    internal static class Session
    {
        private static User _currentUser;

        public static User CurrentUser
        {
            get { return _currentUser; }
        }

        public static bool IsAuthenticated
        {
            get { return _currentUser != null; }
        }

        public static void Start(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            _currentUser = user;
        }

        public static void Logout()
        {
            _currentUser = null;
        }
    }
}