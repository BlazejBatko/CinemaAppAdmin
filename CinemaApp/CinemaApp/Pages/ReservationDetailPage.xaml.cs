using CinemaApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CinemaApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReservationDetailPage : ContentPage
    {
        public ReservationDetailPage(int reservationId)
        {
            InitializeComponent();
            GetReservationDetail(reservationId);
        }

        private async void GetReservationDetail(int reservationId)
        {
            var response = await ApiService.GetReservationDetail(reservationId);
            LblReservationId.Text = response.Id.ToString();
            LblReservationTime.Text = response.ReservationTime.ToString("MMMM d, yyyy HH:mm");
            LblCustomerName.Text = response.CustomerName;
            LblMovieName.Text = response.MovieName;
            LblEmail.Text = response.Email;
            LblPhone.Text = response.Phone;
            LblPrice.Text = response.Price + " zł ";
            LblQty.Text = response.Qty.ToString();
            LblPlayingDate.Text = response.PlayingDate;
            LblPlayingTime.Text = response.PlayingTime;
        }

        private void ImgBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}