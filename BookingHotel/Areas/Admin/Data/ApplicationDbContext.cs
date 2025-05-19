using BookingHotel.Areas.Admin.Models;
using BookingHotel.Models; // Add this to access content models
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Areas.Admin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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
                .HasForeignKey(r => r.EmployeeID);

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

            //Room Service
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

            modelBuilder.Entity<Content_Room>()
    .HasOne(r => r.RoomType)
    .WithMany()
    .HasForeignKey(r => r.RoomTypeID);

            // No additional relationships needed for content entities (they are independent)
        }
    }
}