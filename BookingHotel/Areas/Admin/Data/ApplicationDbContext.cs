using BookingHotel.Areas.Admin.Models;
using BookingHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Add Offers
        public DbSet<Category> Categories { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferHighlight> OfferHighlights { get; set; }
        // Existing DbSet properties for the booking system
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomStatus> RoomStatus { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<BookingStatus> BookingStatus { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<RoomService> RoomServices { get; set; }

        // Add DbSet properties for content-related entities
        public DbSet<Content_BannerImage> Content_BannerImages { get; set; }
        public DbSet<Content_Achivement> Content_Achivements { get; set; }
        public DbSet<Content_Room> Content_Rooms { get; set; }
        public DbSet<Content_Amenity> Content_Amenities { get; set; }
        public DbSet<Content_Offer> Content_Offers { get; set; }
        public DbSet<Content_MembershipBenefit> Content_MembershipBenefits { get; set; }

        // Add DbSet properties for dining-related entities
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RestaurantTag> Restaurant_Tags { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<HeroImage> HeroImages { get; set; }

        // Add DbSet RoomAmenities
        public DbSet<RoomAmenitie> RoomAmenities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Existing relationships for the booking system
            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany()
                .HasForeignKey(r => r.RoomTypeID);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomStatus)
                .WithMany()
                .HasForeignKey(r => r.StatusID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerID);

            modelBuilder.Entity<Booking>()
                .HasOne(r => r.Employee)
                .WithMany()
                .HasForeignKey(r => r.EmployeeID)
                .IsRequired(false);

            modelBuilder.Entity<BookingDetail>()
                .HasOne(r => r.Room)
                .WithMany()
                .HasForeignKey(r => r.RoomID);

            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(bd => bd.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingService>()
                .HasOne(r => r.Service)
                .WithMany()
                .HasForeignKey(r => r.ServiceID);

            modelBuilder.Entity<BookingService>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bd => bd.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(r => r.BookingStatus)
                .WithMany()
                .HasForeignKey(r => r.BookingStatusID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomImage>()
                 .HasOne(r => r.Room)
                 .WithMany(r => r.RoomImages)
                 .HasForeignKey(r => r.RoomID);

            // Room Service
            modelBuilder.Entity<RoomService>()
                .HasKey(rs => new { rs.RoomID, rs.ServiceID });

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Room)
                .WithMany(r => r.RoomServices)
                .HasForeignKey(rs => rs.RoomID);

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Service)
                .WithMany(s => s.RoomServices)
                .HasForeignKey(rs => rs.ServiceID);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.RoomTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationships for dining-related entities
            modelBuilder.Entity<RestaurantTag>()
                .HasKey(rt => new { rt.RestaurantID, rt.TagID });

            modelBuilder.Entity<RestaurantTag>()
                .HasOne(rt => rt.Restaurant)
                .WithMany()
                .HasForeignKey(rt => rt.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RestaurantTag>()
                .HasOne(rt => rt.Tag)
                .WithMany()
                .HasForeignKey(rt => rt.TagID)
                .OnDelete(DeleteBehavior.Cascade);
            //Offers

            // Configure Category entity
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Offers)
                .WithOne(o => o.Category)
                .HasForeignKey(o => o.CategoryId);
            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasColumnType("DATETIME");
            modelBuilder.Entity<Category>()
                .Property(c => c.UpdatedAt)
                .HasColumnType("DATETIME");

            // Configure Offer entity
            modelBuilder.Entity<Offer>()
                .HasMany(o => o.Highlights)
                .WithOne(h => h.Offer)
                .HasForeignKey(h => h.OfferId);
            modelBuilder.Entity<Offer>()
                .Property(o => o.CreatedAt)
                .HasColumnType("DATETIME");
            modelBuilder.Entity<Offer>()
                .Property(o => o.UpdatedAt)
                .HasColumnType("DATETIME");

            // Configure OfferHighlight entity
            modelBuilder.Entity<OfferHighlight>()
                .HasKey(h => h.HighlightId);
            modelBuilder.Entity<OfferHighlight>()
                .Property(h => h.CreatedAt)
                .HasColumnType("DATETIME");
            modelBuilder.Entity<OfferHighlight>()
                .Property(h => h.UpdatedAt)
                .HasColumnType("DATETIME");

            // Configure RoomAmenities entity
            modelBuilder.Entity<RoomAmenitie>()
                .HasKey(rs => new { rs.RoomID, rs.AmenitieID });

            modelBuilder.Entity<RoomAmenitie>()
                .HasOne(rs => rs.Room)
                .WithMany(r => r.RoomAmenities)
                .HasForeignKey(rs => rs.RoomID);

            modelBuilder.Entity<RoomAmenitie>()
                .HasOne(rs => rs.Amenities)
                .WithMany()
                .HasForeignKey(rs => rs.AmenitieID);

            // No additional relationships needed for other content entities
        }
    }
}