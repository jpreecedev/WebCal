﻿using TachographReader.Core;

namespace TachographReader.Views.Settings
{
    public partial class MailSettingsView
    {
        public MailSettingsView()
        {
            InitializeComponent();
            HelpText = Properties.Resources.TXT_EMAIL_SETTINGS_USERS_WONT_SEE_SETTINGS; 
        }
    }
}
