using System;

namespace Models
{
    public class ShopItemModel
    {
        public string ItemId;
        public string Price;
        public string Description;
        public Action Callback;
    }
}