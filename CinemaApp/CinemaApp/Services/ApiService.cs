using CinemaApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnixTimeStamp;
using Xamarin.Essentials;

namespace CinemaApp.Services
{
    //statyczna klasa w celu mozliwosci dostepu do zawartych funkcji w innych czesciach projektu
    public static class ApiService
    {
        public static async Task<bool> RegisterUser(string name, string email, string password)
        {
            var register = new Register()
            {
                Name = name,
                Email = email,
                Password = password
            };
            //Serializacja obiektow na JSON w celu przesyłania do serwera
            var HttpClient = new HttpClient();
            var SerializedJson = JsonConvert.SerializeObject(register);
            var content = new StringContent(SerializedJson, Encoding.UTF8, "application/json");
            //asynchroniczne wysyłanie żadania do enpointu odpowiedzialnego za rejestracje użytkowników
            //Fragment URL pochodzacy z klasy AppSettings w celu poprawy utrzymania kodu
            var response = await HttpClient.PostAsync(AppSettings.ApiUrl + "api/users/register", content);
            //zwracanie wartosci bool w zaleznosci od uzyskanego status code
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public async static Task<bool> Login(string email, string password) 
        {
            var login = new Login()
            {
                Email = email,
                Password = password
            };
            //Serializacja obiektow na JSON w celu przesyłania do serwera
            var HttpClient = new HttpClient();
            var SerializedJson = JsonConvert.SerializeObject(login);
            var content = new StringContent(SerializedJson, Encoding.UTF8, "application/json");
            //asynchroniczne wysyłanie żadania do enpointu odpowiedzialnego za rejestracje użytkowników
            //Fragment URL pochodzacy z klasy AppSettings w celu poprawy utrzymania kodu
            var response = await HttpClient.PostAsync(AppSettings.ApiUrl + "api/users/login", content);
            //zwracanie wartosci bool w zaleznosci od uzyskanego status code
            if (!response.IsSuccessStatusCode) return false;
            var jsonResult = await response.Content.ReadAsStringAsync();
            //Deserializacja w oparciu o własności klasy Token
            var result = JsonConvert.DeserializeObject<Token>(jsonResult);
            //zapisywanie wartosci access tokenu w preferences
            Preferences.Set("accessToken", result.access_token);
            Preferences.Set("userId", result.user_id);
            Preferences.Set("userName", result.user_Name);
            Preferences.Set("tokenExpirationTime", result.expiration_Time);
            Preferences.Set("currentTime", UnixTime.GetCurrentTime());
            return true;
        }

        public static async Task<List<MovieList>> GetAllMovies(int pageNumber, int pageSize)
        {
            //sprawdzenie czy token jest dalej aktualny, jezeli nie to wywolywana jest funkcja wystawiajaca nowy token
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            //pobieranie accessTokenu zapisanego wczesniej w Preferences
            //Wysyłanie headera zawierającego token w celu pobrania listy filmów
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + String.Format("api/movies/AllMovies?sort=asc&pageNumber={0}&pageSize={1}", pageNumber, pageSize));
            //Konwertowanie danych JSON na liste opartą o klasę Movie
            return JsonConvert.DeserializeObject<List<MovieList>>(response);
        }

        public static async Task<bool> DeleteMovie(int movieId)
        {
            //sprawdzenie czy token jest dalej aktualny, jezeli nie to wywolywana jest funkcja wystawiajaca nowy token
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            //pobieranie accessTokenu zapisanego wczesniej w Preferences
            //Wysyłanie headera zawierającego token w celu pobrania szczegółów dotyczących filmu
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrl + String.Format("api/movies/", movieId));
            if (!response.IsSuccessStatusCode) return false;
            return true;
            
        }

        public static async Task<List<Reservation>> GetAllReservations()
        {
            //sprawdzenie czy token jest dalej aktualny, jezeli nie to wywolywana jest funkcja wystawiajaca nowy token
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            //pobieranie accessTokenu zapisanego wczesniej w Preferences
            //Wysyłanie headera zawierającego token w celu pobrania listy filmów zaczynających się wskazanym ciągiem znaków
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + String.Format("api/reservations"));
            //Konwertowanie danych JSON na liste opartą o klasę FindMovie
            return JsonConvert.DeserializeObject<List<Reservation>>(response);
        }

        public static async Task<ReservationDetails> GetReservationDetail(int reservationId)
        {
            //sprawdzenie czy token jest dalej aktualny, jezeli nie to wywolywana jest funkcja wystawiajaca nowy token
            await TokenValidator.CheckTokenValidity();
            var httpClient = new HttpClient();
            //pobieranie accessTokenu zapisanego wczesniej w Preferences
            //Wysyłanie headera zawierającego token w celu pobrania listy filmów zaczynających się wskazanym ciągiem znaków
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + String.Format("api/reservations" + reservationId));
            //Konwertowanie danych JSON na liste opartą o klasę FindMovie
            return JsonConvert.DeserializeObject<ReservationDetails>(response);
        }

        public static async Task<bool> AddMovie(Movie movie)
        {

            //sprawdzenie czy token jest dalej aktualny, jezeli nie to wywolywana jest funkcja wystawiajaca nowy token
            await TokenValidator.CheckTokenValidity();
            //Serializacja obiektow na JSON w celu przesyłania do serwera
            var HttpClient = new HttpClient();

            var content = new MultipartFormDataContent
            {
                {new StringContent(movie.Name),"Name"},
                {new StringContent(movie.Description),"Description"},
                {new StringContent(movie.Language),"Language"},
                {new StringContent(movie.Duration),"Duration"},
                {new StringContent(movie.PlayingDate),"PlayingDate"},
                {new StringContent(movie.PlayingTime),"PlayingTime"},
                {new StringContent(movie.TicketPrice.ToString()),"TicketPrice"},
                {new StringContent(movie.Rating.ToString()),"Rating"},
                {new StringContent(movie.Genre),"Genre"},
                {new StringContent(movie.TrailorUrl),"TrailorUrl"},
            };
            //Header zawierający access token
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("accessToken", string.Empty));
            var response = await HttpClient.PostAsync(AppSettings.ApiUrl + "api/movies", content);
            //zwracanie wartosci bool w zaleznosci od uzyskanego status code
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

    }

    public static class TokenValidator
    {
        public static async Task CheckTokenValidity()
        {
           var expirationTime = Preferences.Get("tokenExpirationTime", 0);
            Preferences.Set("currentTime", UnixTime.GetCurrentTime());
            var currentTime = Preferences.Get("currentTime", 0);
            if (expirationTime < currentTime)
            {
               var email = Preferences.Get("email", string.Empty);
               var password = Preferences.Get("password", string.Empty);

               await ApiService.Login(email, password);

            }

        }
    }
}
