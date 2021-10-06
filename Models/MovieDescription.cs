using System;
using System.ComponentModel.DataAnnotations;

namespace Back_End.Models
{
    public class MovieDescription : IComparable
    {
        [Key]
        public string ImdbID {get; set;}
        public string Title {get; set;}
        public string Year {get; set;}
        public string Type {get; set;}
        public string Poster {get; set;}

        public int CompareTo(object obj)
        {
            if(obj is MovieDescription){
                return this.Title.CompareTo((obj as MovieDescription).Title);
            }
            throw new ArgumentException("Object is not a MovieDescription");
        }
    }
}