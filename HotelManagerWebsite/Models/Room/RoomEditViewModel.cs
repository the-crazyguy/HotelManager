using HotelManagerWebsite.Enums;
using HotelManagerWebsite.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagerWebsite.Models.Room
{
    public class RoomEditViewModel : BaseEditViewModel
    {
        [Required(ErrorMessage = "Please enter a capacity for the room")]
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        public string Type { get; set; }

        #region Fields for the dropdown
        public ICollection<TypePair> RoomTypes { get; set; }
        public int SelectedRoomType { get; set; }
        #endregion

        [Required(ErrorMessage = "Please enter whether the room is available")]
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }

        [Required(ErrorMessage = "Please enter a price for an adult bed for the room")]
        [Display(Name = "Adult Bed Price")]
        public double AdultBedPrice { get; set; }

        [Required(ErrorMessage = "Please enter a price for a child bed for the room")]
        [Display(Name = "Child Bed Price")]
        public double ChildBedPrice { get; set; }

        [Required(ErrorMessage = "Please enter a number for the room")]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }
    }
}
