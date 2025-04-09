using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Linq;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System.Data;
using FinApp5.Conexiones;

namespace FinApp5.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public INavigation? Navigation;

        public event PropertyChangedEventHandler? PropertyChanged;

        private List<Mbarrio>? _barrios;

        public List<Mbarrio> Barrios
        {
            get
            {
                return _barrios;
            }
            set
            {
                _barrios = value;
                OnPropertyChanged(nameof(Barrios));
            }
        }

       

        public List<String> llenarBarrios()
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                List<String> barrio = new List<String>();
                cmd = new SqlCommand("CargarItemsDeBarriosEnGral", CONEXIONMAESTRA.conectar);
                CONEXIONMAESTRA.Abrir();

                cmd.CommandType = CommandType.StoredProcedure;

                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        barrio.Add(rdr["rbcNombre"].ToString());
                    }
                }
                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();
                return barrio;
            }
            catch (Exception)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();
                return null;
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();
            }

        }

        private Mbarrio selectedBarrio;

        public Mbarrio SelectedBarrio
        {
            get
            {
                return selectedBarrio;
            }
            set
            {
                if (selectedBarrio != value)
                {
                    selectedBarrio = value;
                    OnPropertyChanged(nameof(SelectedBarrio));
                }
            }
        }

        private ImageSource? foto;
        public ImageSource? Foto
        {
            get { return foto; }
            set
            {
                foto = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        private string? _title;
        public string? Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                
            }
        }

        //public bool IsBusy
        //{
        //    get { return _isBusy; }
        //    set
        //    {
        //        SetProperty(ref _isBusy, value);
        //    }
        //}
        protected void SetValue<T>(ref T backingFieled, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingFieled, value))
            {
                return;
            }
            backingFieled = value;
            OnPropertyChanged(propertyName);
        }

        public string PrimerletraMayus(string objeto)
        {
            try
            {
                string input = objeto.ToString().ToLower() ?? throw new Exception();
                return input.First().ToString().ToUpper()+input.Substring(1);
            }
            catch 
            {
                return string.Empty;
            }
        }

    }
}