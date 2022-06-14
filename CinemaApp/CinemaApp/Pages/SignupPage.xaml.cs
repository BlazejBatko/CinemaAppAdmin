using CinemaApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CinemaApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        public SignupPage()
        {
            InitializeComponent();
        }

        private  async void ImgSignup_Tapped(object sender, EventArgs e)
        {
            var email = EntEmail.Text;
            var password = EntPassword.Text;
            var name = EntName.Text;
            var emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            //5 znakow, minimum 1 cyfra, minimum 1 litera
            var passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$";
           // var namePattern = @"/^[a-z ,.'-]+$/i";
            var response = await ApiService.RegisterUser(EntName.Text, EntEmail.Text, EntPassword.Text);
            if (response && Regex.IsMatch(email, emailPattern) && Regex.IsMatch(password, passwordPattern) )
            {
                await DisplayAlert("Hej", "Konto zostało stworzone pomyślnie", "Kontynuuj");
                await Navigation.PushModalAsync(new LoginPage());
            }
            else if (!Regex.IsMatch(email, emailPattern))
            {
                // DisplayAlert("💥", "Niepoprawny format adresu email", "Anuluj");
                InvEmail.Text = "Niepoprawny format adresu email";
            }
            
            else if (!Regex.IsMatch(password, passwordPattern))
            {
                
                InvEmail.Text = "haslo powinno zawierac minimum 5 znakow i 1 cyfre";
            }
            else
            {
                await DisplayAlert(":(", "Niestety nie udało się utworzyć konta", "Anuluj");
            }
        }

        private async void LblLogin_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginPage());

        }
    }
}