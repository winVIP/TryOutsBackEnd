using System;
using System.Web;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_End.Models;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public MovieController(MovieContext context, IHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        // GET: api/Movie/Search/{MovieName}
        [Route("Search/{movieName}")]
        [HttpGet]
        public async Task<ActionResult<MovieDescription[]>> GetMovieDescriptions(string movieName)
        {
            var movieDescriptions = await _context.MovieDescriptions.Where(movie => movie.Title.ToLower().Contains((HttpUtility.UrlDecode(movieName).ToLower()))).ToArrayAsync();

            if(movieDescriptions.Length == 0){
                var HttpClient = _clientFactory.CreateClient();

                var request = new HttpRequestMessage(HttpMethod.Get, "http://www.omdbapi.com/?i=tt3896198&apikey=bd0f094&s=" + HttpUtility.UrlEncode(movieName));

                var response = await HttpClient.SendAsync(request);

                var responseString = await response.Content.ReadAsStringAsync();

                movieDescriptions = null;

                using(JsonDocument JSONresponse = JsonDocument.Parse(responseString)){
                    JsonElement root = JSONresponse.RootElement;

                    if(root.GetProperty("Response").GetString() == "False"){
                        return NotFound();
                    }

                    int amount = int.Parse(root.GetProperty("totalResults").GetString());
                    movieDescriptions = new MovieDescription[amount];

                    var searchResults = root.GetProperty("Search").EnumerateArray();

                    for (int i = 0; i < searchResults.Count(); i++){
                        MovieDescription newMovie = new MovieDescription();
                        var currentElement = searchResults.ElementAt(i);
                        newMovie.ImdbID = currentElement.GetProperty("imdbID").GetString();
                        newMovie.Title = currentElement.GetProperty("Title").GetString();
                        newMovie.Year = currentElement.GetProperty("Year").GetString();
                        newMovie.Type = currentElement.GetProperty("Type").GetString();
                        newMovie.Poster = currentElement.GetProperty("Poster").GetString();
                        movieDescriptions[i] = newMovie;
                        _context.MovieDescriptions.Add(newMovie);
                    }
                    
                    _context.SaveChanges();
                }
            }

            Array.Sort(movieDescriptions);

            return movieDescriptions;
        }

        // GET: api/Movie/{ImdbID}
        [HttpGet("{ImdbID}")]
        public async Task<ActionResult<MovieFullDescription>> GetFullMovieDescription(string ImdbID){
            var HttpClient = _clientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://www.omdbapi.com/?apikey=bd0f094&plot=short&i=" + HttpUtility.UrlEncode(ImdbID));

            var response = await HttpClient.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();

            using(JsonDocument JSONresponse = JsonDocument.Parse(responseString)){
                JsonElement root = JSONresponse.RootElement;

                if(root.GetProperty("Response").GetString() == "False"){
                    return NotFound();
                }

                MovieFullDescription movie = new MovieFullDescription();
                movie.Title = root.GetProperty("Title").GetString();
                movie.Year = root.GetProperty("Year").GetString();
                movie.imdbRating = root.GetProperty("imdbRating").GetString();
                movie.Genre = root.GetProperty("Genre").GetString();
                movie.Plot = root.GetProperty("Plot").GetString();
                movie.Actors = root.GetProperty("Title").GetString();

                return movie;
            }
        }
    }
}
