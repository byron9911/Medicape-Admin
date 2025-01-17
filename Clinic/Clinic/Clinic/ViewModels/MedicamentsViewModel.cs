﻿using Clinic.Clases;
using Clinic.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace Clinic.ViewModels
{
    public class MedicamentsViewModel : BaseViewModel
    {
        Connection get = new Connection();
        Functions functions;

        #region Propiedades
        ObservableCollection<Medicamentos> _Items;
        public ObservableCollection<Medicamentos> Items
        {
            get { return _Items; }
            set { SetValue(ref _Items, value); }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetValue(ref _isRefreshing, value); }
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetValue(ref _isVisible, value); }
        }

        private string _query;
        public string Query
        {
            get { return _query; }
            set { SetValue(ref _query, value); }
        }

        private bool _noresults;
        public bool NoResults
        {
            get { return _noresults; }
            set { SetValue(ref _noresults, value); }
        }

        private bool _listVisible;
        public bool ListVisible
        {
            get { return _listVisible; }
            set { SetValue(ref _listVisible, value); }
        }
        #endregion

        #region Constructor
        public MedicamentsViewModel()
        {
            IsVisible = false;
            ListVisible = true;
            functions = new Functions();
            GetMedicaments();
        }
        #endregion

        private async void GetMedicaments()
        {
            var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Cargando...");
            bool result = get.TestConnection();
            if (result == true)
            {
                IsVisible = false;
                ListVisible = true;
                var response = await functions.Read<Medicamentos>("/Api/medicamentos/read.php");
                if (!response.IsSuccess)
                {
                    await loadingDialog.DismissAsync();
                    await MaterialDialog.Instance.AlertAsync(message: response.Message,
                                               title: "Error",
                                               acknowledgementText: "Ok");
                }
                else if (response.Result == null)
                {
                    await loadingDialog.DismissAsync();
                    await MaterialDialog.Instance.AlertAsync(message: response.Message,
                                               title: "Error",
                                               acknowledgementText: "Ok");
                }
                else
                {
                    await loadingDialog.DismissAsync();
                    var list = (List<Medicamentos>)response.Result;
                    Items = new ObservableCollection<Medicamentos>(list);
                }
            }
            else
            {
                await loadingDialog.DismissAsync();
                IsVisible = true;
                ListVisible = false;
            }
        }

        #region Comandos
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(() =>
                {
                    IsRefreshing = true;

                    GetMedicaments();

                    IsRefreshing = false;
                });
            }
        }


        public ICommand Reconnect
        {
            get
            {
                return new RelayCommand(GetMedicaments);
            }
        }
        #endregion
    }
}
