using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace belt.Models
{
    public class User
    {
        [Key]
        public int UserId {get; set;}


        [Display(Name="First Name:")]
        [MinLength(2,ErrorMessage="Dude your FirstName is longer than 2 characters....commmon!")]
        [Required(ErrorMessage="FirstName is a must Yo!!!")]
        public string FirstName{get;set;}


        [Display(Name="Last Name:")]
        [MinLength(2,ErrorMessage="Dude your LastName is longer than 2 characters....commmon!")]
        [Required(ErrorMessage="LastName is a must Yo!!!")]
        public string LastName{get;set;}


        [Display(Name="Email:")]
        [EmailAddress(ErrorMessage="Use the proper email format yo! It is 2019 for godsake!!!")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Required(ErrorMessage="Email is a must sorry :(")]
        public string Email{get;set;}

        
        [Display(Name="Password:")]
        [RegularExpression("^.*(?=.{8,})(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$", ErrorMessage="Password needs to have at least 1 Lower case, 1 Upper case, I Special character,1 Number   ")]
        [MinLength(8,ErrorMessage="Password needs to be more than 8 characters please")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage="Of course password is required?")]
        public string Password{get;set;}


        [NotMapped]
        [Display(Name="Confirm Password:")]
        [Compare(nameof(Password), ErrorMessage="Password don't match.")]
        [Required(ErrorMessage="Password Confirmation is required please!")]
        public string RePassword{get;set;}

        public DateTime CreatedAt{get;set;}=DateTime.Now;
        public DateTime UpdatedAt{get;set;}=DateTime.Now;

        //Nagivation properties for related Message anf Comment objects
        public List<Attendence> Events {get;set;}
    }

    public class Event
    {
        [Key]
        public int EventId {get;set;}
        [Required(ErrorMessage="Title is a Must!")]
        public string Title {get;set;}

        [Required(ErrorMessage="DateTime is a Must!")]
        [FutureDate]
        public DateTime EventDate{get;set;}
        
        [Required(ErrorMessage="Duration is a Must!")]
        [Range(0,1000)]
        public Decimal Duration{get;set;}

        [Required(ErrorMessage="Description is a Must!")]
        public string Description{get;set;}
        public int UserId{get;set;}
        public User Coordinator {get;set;}
        public DateTime CreatedAt{get;set;}=DateTime.Now;
        public DateTime UpdatedAt{get;set;}=DateTime.Now;
        public List<Attendence> Participants {get;set;}
    }

    public class Attendence
    {
        [Key]
        public int AttendId{get;set;}
        [Required]
        public int UserId {get;set;}
        public User User{get;set;}
        [Required]
        public int EventId{get;set;}
        public Event Event{get;set;}

    }
    public class Login
    {
        [Display(Name="Email:")]
        [EmailAddress(ErrorMessage="Use the proper email format yo! It is 2019 for godsake!!!")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Required(ErrorMessage="Email is a must sorry :(")]
        public string Email{get;set;}

        [Display(Name="Password:")]
        [Required(ErrorMessage="Of course password is required?")]
        public string Password{get;set;}
    }   
    
     public class IndexViewModel
    {
        public Login NewLogin {get;set;}
        public User NewUser {get;set;}
    }

}

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // DateTime Current = DateTime.Now;
        if( (DateTime)value<DateTime.Now)
        {
            return new ValidationResult("Wedding planned must be a future date");
        }
        return ValidationResult.Success;
        // You first may want to unbox "value" here and cast to to a DateTime variable!
    }
}