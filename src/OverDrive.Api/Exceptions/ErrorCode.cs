namespace OverDrive.Api.Exceptions;

public enum ErrorCode
{
    Unknown,
    Customer_NotFound,
    Customer_Inactive,
    ItemType_Invalid,
    Item_QuantityInvalid,
    Item_DiscountNegative,
    Item_DiscountTooLarge,
    Part_NotFound,
    Part_InsufficientStock,
    Vehicle_NotFound,
    Vehicle_AlreadyOwned,
    Vehicle_AlreadySold,
    Sale_TotalNegative,
    Sale_MissingRequiredFields,
    Sale_Blocked,
    Vehicle_InvalidQuantity,
    SaleItem_NotFound
}