namespace NiksoftCore.Utilities
{
    public static class FoodTools
    {
        public static long GetTax(this long price)
        {
            return (100 * (price / 109));
        }
    }
}
