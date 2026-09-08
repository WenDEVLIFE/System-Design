namespace System_Design
{
    internal sealed class Dashboard
    {
        public void Show()
        {
            using (DashboardForm form = new DashboardForm())
            {
                form.ShowDialog();
            }
        }
    }
}