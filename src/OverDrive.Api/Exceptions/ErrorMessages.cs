namespace OverDrive.Api.Exceptions;

public static class ErrorMessages
{
    public static string GetMessage(ErrorCode code)
    {
        return code switch
        {
            ErrorCode.Customer_NotFound => "Customer not found.",
            ErrorCode.Customer_Inactive => "Customer is inactive.",

            ErrorCode.ItemType_Invalid => "ItemType must be PART or VEHICLE.",
            ErrorCode.Item_QuantityInvalid => "Item quantity must be greater than zero.",
            ErrorCode.Item_DiscountNegative => "Item discount must be non-negative.",
            ErrorCode.Item_DiscountTooLarge => "Item discount cannot be greater than unit price.",

            ErrorCode.Part_NotFound => "Part not found.",
            ErrorCode.Part_InsufficientStock => "Insufficient part quantity.",

            ErrorCode.Vehicle_NotFound => "Vehicle not found.",
            ErrorCode.Vehicle_AlreadyOwned => "Customer already owns this vehicle.",
            ErrorCode.Vehicle_AlreadySold => "Vehicle already sold.",

            ErrorCode.Sale_TotalNegative => "Total cannot be negative.",
            ErrorCode.Sale_MissingRequiredFields => "Required sale fields not informed.",
            ErrorCode.Sale_Blocked => "Sale cannot be edited in its current status.",
            ErrorCode.SaleItem_NotFound => "Sale item not found.",
            ErrorCode.Vehicle_InvalidQuantity => "Vehicle item quantity must be 1 and cannot be changed.",

            _ => "Business error."
        };
    }
}
