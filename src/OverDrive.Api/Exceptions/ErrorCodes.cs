namespace OverDrive.Api.Exceptions;

public static class ErrorCodes
{
    public static class Customer
    {
        public const ErrorCode NotFound = ErrorCode.Customer_NotFound;
        public const ErrorCode Inactive = ErrorCode.Customer_Inactive;
    }

    public static class Item
    {
        public const ErrorCode InvalidType = ErrorCode.ItemType_Invalid;
        public const ErrorCode QuantityInvalid = ErrorCode.Item_QuantityInvalid;
        public const ErrorCode DiscountNegative = ErrorCode.Item_DiscountNegative;
        public const ErrorCode DiscountTooLarge = ErrorCode.Item_DiscountTooLarge;
    }

    public static class Part
    {
        public const ErrorCode NotFound = ErrorCode.Part_NotFound;
        public const ErrorCode NoStock = ErrorCode.Part_InsufficientStock;
    }

    public static class Vehicle
    {
        public const ErrorCode NotFound = ErrorCode.Vehicle_NotFound;
        public const ErrorCode AlreadyOwned = ErrorCode.Vehicle_AlreadyOwned;
        public const ErrorCode AlreadySold = ErrorCode.Vehicle_AlreadySold;
        public const ErrorCode InvalidQuantity = ErrorCode.Vehicle_InvalidQuantity;
    }

    public static class Sale
    {
        public const ErrorCode TotalNegative = ErrorCode.Sale_TotalNegative;
        public const ErrorCode MissingRequiredFields = ErrorCode.Sale_MissingRequiredFields;
        public const ErrorCode Blocked = ErrorCode.Sale_Blocked;
    }

    public static class SaleItem
    {
        public const ErrorCode NotFound = ErrorCode.SaleItem_NotFound;
    }
}
