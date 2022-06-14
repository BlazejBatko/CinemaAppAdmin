﻿using ImageToArray;
using Plugin.Media;
using Plugin.Media.Abstractions;
using CinemaApp.Models;
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
    public partial class AddMoviePage : ContentPage
    {
        private MediaFile file;
        public AddMoviePage()
        {
            InitializeComponent();
        }

        private async void TapPickImage_Tapped(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert(":(", "Niestety twoje urządzenie nie wspiera danej funkcji", "Anuluj");
                return;
            }

            file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;


            ImgMovie.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });
        }

        private async void ImgSave_Tapped(object sender, EventArgs e)
        {
            //konwertowanie Stream na bytową tablice
            var imageArray = FromFile.ToArray(file.GetStream());
            var movie = new Movie()
            {
                Name = EntMovieName.Text,
                Description = EdtDescription.Text,
                Language = EntLanguage.Text,
                Duration = EntDuration.Text,
                PlayingDate = EntPlayingDate.Text,
                PlayingTime = EntPlayingTime.Text,
                TicketPrice = Convert.ToInt32(EntTicketPrice.Text),
                Rating = Convert.ToDouble(EntRating.Text),
                Genre = EntGenre.Text,
                TrailorUrl = EntTrailorUrl.Text,
                ImageArray = imageArray
            };

            var response = await ApiService.AddMovie(file, movie);
            if (!response)
            {
                await DisplayAlert(":(", "Niestety nie udało się dodać filmu", "Anuluj");
            }
            else
            {
                await DisplayAlert(":)", "Film został dodany", "Kontynuuj");
                await Navigation.PopModalAsync();
            }
        }

        private void ImgBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}