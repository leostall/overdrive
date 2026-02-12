using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OverDrive.Api.Models;

public partial class OverDriveDbContext : DbContext
{
    public OverDriveDbContext(DbContextOptions<OverDriveDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleItem> SaleItems { get; set; }

    public virtual DbSet<ServiceOrder> ServiceOrders { get; set; }

    public virtual DbSet<ServiceOrderItem> ServiceOrderItems { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<UsePart> UseParts { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__address__3213E83F40FA4832");

            entity.ToTable("address", tb => tb.HasTrigger("tr_update_address"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Street)
                .HasColumnName("street")
                .HasMaxLength(120)
                .IsUnicode(false);

            entity.Property(e => e.Number)
                .HasColumnName("number")
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Complement)
                .HasColumnName("complement")
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.Property(e => e.Neighborhood)
                .HasColumnName("neighborhood")
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.Property(e => e.City)
                .HasColumnName("city")
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.Property(e => e.State)
                .HasColumnName("state")
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Zip)
                .HasColumnName("zip")
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");
        });


        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__branch__3213E83F19101737");

            entity.ToTable("branch", tb => tb.HasTrigger("tr_update_branch"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.CorporateName)
                .HasColumnName("corporate_name")
                .HasMaxLength(80);

            entity.Property(e => e.Cnpj)
                .HasColumnName("cnpj")
                .HasMaxLength(14)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Phone)
                .HasColumnName("phone")
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Active)
                .HasColumnName("active");

            entity.Property(e => e.FkAddress)
                .HasColumnName("fk_address");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkAddressNavigation)
                .WithMany(p => p.Branches)
                .HasForeignKey(d => d.FkAddress)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__branch__fk_addre__5CD6CB2B");
        });


        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__customer__3213E83FA4E22154");

            entity.ToTable("customer", tb => tb.HasTrigger("tr_update_customer"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.CpfCnpj)
                .HasColumnName("cpf_cnpj")
                .HasMaxLength(14)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.CustomerType)
                .HasColumnName("customer_type")
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100);

            entity.Property(e => e.Phone)
                .HasColumnName("phone")
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Active)
                .HasColumnName("active");

            entity.Property(e => e.FkAddress)
                .HasColumnName("fk_address");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkAddressNavigation)
                .WithMany(p => p.Customers)
                .HasForeignKey(d => d.FkAddress)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__customer__fk_add__5441852A");
        });


        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__departme__3213E83F02B2AE8A");

            entity.ToTable("department", tb => tb.HasTrigger("tr_update_department"));

            entity.HasIndex(e => e.Name, "UX_department_name")
                .IsUnique();

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.Property(e => e.Active)
                .HasColumnName("active")
                .HasDefaultValue(true, "DF_department_active");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(sysutcdatetime())", "DF_department_created_at");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at");
        });


        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__employee__3213E83FF4B6F274");

            entity.ToTable("employee", tb => tb.HasTrigger("tr_update_employee"));

            entity.HasIndex(e => e.FkDepartment, "IX_employee_fk_department");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Registration)
                .HasColumnName("registration")
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100);

            entity.Property(e => e.CommissionRate)
                .HasColumnName("commission_rate")
                .HasColumnType("decimal(5, 4)");

            entity.Property(e => e.Active)
                .HasColumnName("active");

            entity.Property(e => e.BirthDate)
                .HasColumnName("birth_date")
                .HasColumnType("datetime");

            entity.Property(e => e.FkAddress)
                .HasColumnName("fk_address");

            entity.Property(e => e.FkBranch)
                .HasColumnName("fk_branch");

            entity.Property(e => e.FkDepartment)
                .HasColumnName("fk_department");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkAddressNavigation)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.FkAddress)
                .HasConstraintName("FK__employee__fk_add__656C112C");

            entity.HasOne(d => d.FkBranchNavigation)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.FkBranch)
                .HasConstraintName("FK__employee__fk_bra__66603565");

            entity.HasOne(d => d.FkDepartmentNavigation)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.FkDepartment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_employee_department");
        });


        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__part__3213E83F17C1B235");

            entity.ToTable("part", tb => tb.HasTrigger("tr_update_part"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Code)
                .HasColumnName("code")
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.FkStock)
                .HasColumnName("fk_stock");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkStockNavigation)
                .WithMany(p => p.Parts)
                .HasForeignKey(d => d.FkStock)
                .HasConstraintName("FK__part__fk_stock__6A30C649");
        });


        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__payment__3213E83FF17B5BD5");

            entity.ToTable("payment", tb => tb.HasTrigger("tr_update_payment"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.PaymentMethod)
                .HasColumnName("payment_method")
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");
        });


        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__sale__3213E83F4A81A4F0");

            entity.ToTable("sale", tb => tb.HasTrigger("tr_update_sale"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.SaleDate)
                .HasColumnName("sale_date")
                .HasColumnType("datetime");

            entity.Property(e => e.Subtotal)
                .HasColumnName("subtotal")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Discount)
                .HasColumnName("discount")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.AdditionalFee)
                .HasColumnName("additional_fee")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Total)
                .HasColumnName("total")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.FkCustomer)
                .HasColumnName("fk_customer");

            entity.Property(e => e.FkBranch)
                .HasColumnName("fk_branch");

            entity.Property(e => e.FkEmployee)
                .HasColumnName("fk_employee");

            entity.Property(e => e.FkPayment)
                .HasColumnName("fk_payment");

            entity.Property(e => e.FkStatus)
                .HasColumnName("fk_status");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkBranchNavigation)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.FkBranch)
                .HasConstraintName("FK__sale__fk_branch__6EF57B66");

            entity.HasOne(d => d.FkCustomerNavigation)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.FkCustomer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sale__fk_custome__6E01572D");

            entity.HasOne(d => d.FkEmployeeNavigation)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.FkEmployee)
                .HasConstraintName("FK__sale__fk_employe__6FE99F9F");

            entity.HasOne(d => d.FkPaymentNavigation)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.FkPayment)
                .HasConstraintName("FK__sale__fk_payment__70DDC3D8");

            entity.HasOne(d => d.FkStatusNavigation)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.FkStatus)
                .HasConstraintName("FK__sale__fk_status__71D1E811");
        });


        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__sale_ite__3213E83F17FBE7CE");

            entity.ToTable("sale_item", tb => tb.HasTrigger("tr_update_sale_item"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.ItemType)
                .HasColumnName("item_type")
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");

            entity.Property(e => e.UnitPrice)
                .HasColumnName("unit_price")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Discount)
                .HasColumnName("discount")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.FkPart)
                .HasColumnName("fk_part");

            entity.Property(e => e.FkVehicle)
                .HasColumnName("fk_vehicle");

            entity.Property(e => e.FkSale)
                .HasColumnName("fk_sale");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkPartNavigation)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(d => d.FkPart)
                .HasConstraintName("FK__sale_item__fk_pa__76969D2E");

            entity.HasOne(d => d.FkSaleNavigation)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(d => d.FkSale)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__sale_item__fk_sa__75A278F5");

            entity.HasOne(d => d.FkVehicleNavigation)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(d => d.FkVehicle)
                .HasConstraintName("FK__sale_item__fk_ve__778AC167");
        });


        modelBuilder.Entity<ServiceOrder>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__service___3213E83F4FC6858D");

            entity.ToTable("service_order", tb => tb.HasTrigger("tr_update_service_order"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Number)
                .HasColumnName("number")
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.OpenDate)
                .HasColumnName("open_date")
                .HasColumnType("datetime");

            entity.Property(e => e.CloseDate)
                .HasColumnName("close_date")
                .HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.TotalValue)
                .HasColumnName("total_value")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.FkVehicle)
                .HasColumnName("fk_vehicle");

            entity.Property(e => e.FkBranch)
                .HasColumnName("fk_branch");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkBranchNavigation)
                .WithMany(p => p.ServiceOrders)
                .HasForeignKey(d => d.FkBranch)
                .HasConstraintName("FK__service_o__fk_br__7C4F7684");

            entity.HasOne(d => d.FkVehicleNavigation)
                .WithMany(p => p.ServiceOrders)
                .HasForeignKey(d => d.FkVehicle)
                .HasConstraintName("FK__service_o__fk_ve__7B5B524B");
        });


        modelBuilder.Entity<ServiceOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__service___3213E83FD5F1FAF0");

            entity.ToTable("service_order_item", tb => tb.HasTrigger("tr_update_service_order_item"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Subtotal)
                .HasColumnName("subtotal")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.FkServiceOrder)
                .HasColumnName("fk_service_order");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkServiceOrderNavigation)
                .WithMany(p => p.ServiceOrderItems)
                .HasForeignKey(d => d.FkServiceOrder)
                .HasConstraintName("FK__service_o__fk_se__00200768");
        });


        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__status__3213E83F9F27654C");

            entity.ToTable("status", tb => tb.HasTrigger("tr_update_status"));

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");
        });


        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__stock__3213E83F897B00AF");

            entity.ToTable("stock", tb => tb.HasTrigger("tr_update_stock"));

            entity.HasIndex(e => e.FkAddress, "IX_stock_fk_address");
            entity.HasIndex(e => e.FkBranch, "IX_stock_fk_branch");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Note)
                .HasColumnName("note")
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.FkAddress)
                .HasColumnName("fk_address");

            entity.Property(e => e.FkBranch)
                .HasColumnName("fk_branch");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkAddressNavigation)
                .WithMany(p => p.Stocks)
                .HasForeignKey(d => d.FkAddress)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__stock__fk_addres__60A75C0F");

            entity.HasOne(d => d.FkBranchNavigation)
                .WithMany(p => p.Stocks)
                .HasForeignKey(d => d.FkBranch)
                .HasConstraintName("FK__stock__fk_branch__619B8048");
        });


        modelBuilder.Entity<UsePart>(entity =>
        {
            entity.HasKey(e => new { e.FkPart, e.FkServiceOrderItem })
                .HasName("PK__use_part__1CCB9A42DAA6906A");

            entity.ToTable("use_part");

            entity.Property(e => e.FkPart)
                .HasColumnName("fk_part");

            entity.Property(e => e.FkServiceOrderItem)
                .HasColumnName("fk_service_order_item");

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");

            entity.HasOne(d => d.FkPartNavigation)
                .WithMany(p => p.UseParts)
                .HasForeignKey(d => d.FkPart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__use_part__fk_par__02FC7413");

            entity.HasOne(d => d.FkServiceOrderItemNavigation)
                .WithMany(p => p.UseParts)
                .HasForeignKey(d => d.FkServiceOrderItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__use_part__fk_ser__03F0984C");
        });


        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__vehicle__3213E83F49B64BFF");

            entity.ToTable("vehicle", tb => tb.HasTrigger("tr_update_vehicle"));

            entity.HasIndex(e => e.FkVehicleType, "IX_vehicle_fk_vehicle_type");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Chassi)
                .HasColumnName("chassi")
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.Plate)
                .HasColumnName("plate")
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Brand)
                .HasColumnName("brand")
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Model)
                .HasColumnName("model")
                .HasMaxLength(80)
                .IsUnicode(false);

            entity.Property(e => e.Year)
                .HasColumnName("year");

            entity.Property(e => e.Mileage)
                .HasColumnName("mileage");

            entity.Property(e => e.Condition)
                .HasColumnName("condition")
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Value)
                .HasColumnName("value")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.FkAddress)
                .HasColumnName("fk_address");

            entity.Property(e => e.FkCustomer)
                .HasColumnName("fk_customer");

            entity.Property(e => e.FkVehicleType)
                .HasColumnName("fk_vehicle_type");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FkAddressNavigation)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.FkAddress)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vehicle__fk_addr__5812160E");

            entity.HasOne(d => d.FkCustomerNavigation)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.FkCustomer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__vehicle__fk_cust__59063A47");

            entity.HasOne(d => d.FkVehicleTypeNavigation)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.FkVehicleType)
                .HasConstraintName("FK_vehicle_vehicle_type");
        });


        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__vehicle___3213E83F38595630");

            entity.ToTable("vehicle_type", tb => tb.HasTrigger("tr_update_vehicle_type"));

            entity.HasIndex(e => e.Name, "UX_vehicle_type_name")
                .IsUnique();

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.WheelCount)
                .HasColumnName("wheel_count");

            entity.Property(e => e.Active)
                .HasColumnName("active")
                .HasDefaultValue(true, "DF_vehicle_type_active");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("(sysutcdatetime())", "DF_vehicle_type_created_at");

            entity.Property(e => e.EditedAt)
                .HasColumnName("edited_at");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
