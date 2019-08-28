namespace BookStore.Models
{
   public partial class Cart
    {
        #region "Cart"
        public int Quantity
        {
            get;
            set;
        }
        public virtual BookDetail BookDetail { get; set; }
        #endregion
    }
}