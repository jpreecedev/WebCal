﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Webcal.Core;
using Webcal.Properties;
using Webcal.Views.Settings;

namespace Webcal.Views
{
    public class SettingsViewModel : BaseNavigationViewModel
    {
        #region Constructor

        public SettingsViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            ShowSettingsView(typeof(GeneralSettingsView));
            BuildTreeView();
        }

        #endregion

        #region Public Properties

        public bool IsPromptVisible { get; set; }

        public string Prompt { get; set; }

        public Action<string> Callback { get; set; }

        public ObservableCollection<TreeViewItem> TreeViewItems { get; set; }

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            ItemClickedCommand = new DelegateCommand<Type>(OnItemClicked);
        }

        #endregion

        #region Public Methods

        public void SetSelectedTreeViewItem(Type type)
        {
            if (type == null)
                return;

            SetSelectedItem(type, TreeViewItems.ToList());
        }

        #endregion

        #region Commands

        #region Command : Item Clicked

        public DelegateCommand<Type> ItemClickedCommand { get; set; }

        private void OnItemClicked(Type param)
        {
            ShowSettingsView(param);
        }

        #endregion

        #endregion

        #region Private Methods

        private static void SetSelectedItem(Type type, IEnumerable<TreeViewItem> items)
        {
            if (type == null || items == null)
                return;

            foreach (TreeViewItem item in items)
            {
                item.IsSelected = Equals(item.DataContext, type);

                if (item.Items != null)
                    SetSelectedItem(type, item.Items.Cast<TreeViewItem>().ToList());
            }
        }

        private void BuildTreeView()
        {
            TreeViewItems = new ObservableCollection<TreeViewItem>
                                {
                                    new TreeViewItem
                                        {
                                            IsSelected = true,
                                            IsExpanded = true,
                                            DataContext = typeof (GeneralSettingsView),
                                            Header = Resources.TXT_GENERAL_SETTINGS,
                                            ItemsSource = new List<TreeViewItem>
                                                              {
                                                                  new TreeViewItem {Header = Resources.TXT_WORKSHOP_SETTINGS, DataContext = typeof (WorkshopSettingsView)},
                                                                  new TreeViewItem {Header = Resources.TXT_REGISTRATION_SETTINGS, DataContext = typeof (RegistrationSettingsView)}
                                                              }
                                        },
                                    new TreeViewItem {Header = Resources.TXT_CUSTOMER_SETTINGS, DataContext = typeof (CustomerSettingsView)},
                                    new TreeViewItem
                                        {
                                            Header = Resources.TXT_SHORTCUT_SETTINGS,
                                            DataContext = typeof (ShortcutSettingsView),
                                            IsExpanded = true,
                                            ItemsSource = new List<TreeViewItem>
                                                              {
                                                                  new TreeViewItem {Header = Resources.TXT_TACHOGRAPH_MAKES_MODELS, DataContext = typeof (TachographMakesModelsView)},
                                                                  new TreeViewItem {Header = Resources.TXT_VEHICLE_MAKES_MODELS, DataContext = typeof (VehicleMakesModelsView)},
                                                                  new TreeViewItem {Header = Resources.TXT_TECHNICIAN, DataContext = typeof (TechniciansView)},
                                                                  new TreeViewItem {Header = Resources.TXT_INSPECTION_METHODS, DataContext = typeof (InspectionMethodsView)},
                                                                  new TreeViewItem {Header = Resources.TXT_INSPECTION_EQUIPMENTS, DataContext = typeof (InspectionEquipmentsView)},
                                                                  new TreeViewItem {Header = Resources.TXT_TYRE_SIZES, DataContext = typeof (TyreSizesView)}
                                                              }
                                        },
                                    new TreeViewItem {Header = Resources.TXT_PRINTER_SETTINGS, DataContext = typeof (PrinterSettingsView)},
                                    new TreeViewItem {Header = Resources.TXT_USER_MANAGEMENT, DataContext = typeof (UserManagementView)},
                                    new TreeViewItem { Header = Resources.TXT_MAIL_SETTINGS, DataContext = typeof(MailSettingsView)}
                                };
        }

        #endregion
    }
}