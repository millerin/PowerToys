﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows;
using FileActionsMenu.Helpers;
using FileActionsMenu.Interfaces;
using FileActionsMenu.Ui.Helpers;
using Wpf.Ui.Controls;

namespace PowerToys.FileActionsMenu.Plugins.MoveCopyActions
{
    internal sealed class NewFolderWithSelection : IAction
    {
        private string[]? _selectedItems;

        public string[] SelectedItems { get => _selectedItems.GetOrArgumentNullException(); set => _selectedItems = value; }

        public string Header => "New folder with selection";

        public IAction.ItemType Type => IAction.ItemType.SingleItem;

        public IAction[]? SubMenuItems => null;

        public int Category => 1;

        public IconElement? Icon => new FontIcon { Glyph = "\uE8F4" };

        public bool IsVisible => true;

        public Task Execute(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Path.GetDirectoryName(SelectedItems[0]) ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "New folder with selection");

            int i = 0;
            while (Directory.Exists(path))
            {
                if (path.EndsWith(')'))
                {
                    path = path[..^(3 + i.ToString(CultureInfo.InvariantCulture).Length)];
                }

                i++;
                path += " (" + i + ")";
            }

            Directory.CreateDirectory(path);

            FileActionProgressHelper fileActionProgressHelper = new();
            fileActionProgressHelper.SetTitle("Moving files to new folder");
            fileActionProgressHelper.SetTotal(SelectedItems.Length);

            foreach (string item in SelectedItems)
            {
                fileActionProgressHelper.SetCurrentObjectName(Path.GetFileName(item));

                File.Move(item, Path.Combine(Path.GetDirectoryName(SelectedItems[0]) ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "New folder with selection", Path.GetFileName(item)));
            }

            fileActionProgressHelper.Close();

            return Task.CompletedTask;
        }
    }
}