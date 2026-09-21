namespace System_Design
{
    internal sealed class Dashboard
    {
        public void Show()
        {
            using (Form2 form = new Form2())
            {
                form.ShowDialog();
            }
        }
    }
}