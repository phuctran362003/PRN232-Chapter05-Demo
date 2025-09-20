using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository.Repositories
{

    public class UserAccountRepo : DataAccessObject<UserAccount>
    {
        public UserAccountRepo(WatercolorsPainting2024DbContext context) : base(context) { }

        public async Task<UserAccount> GetByEmailAndPassword(string email, string password)
        {
            return await _context.UserAccounts.FirstOrDefaultAsync(a => a.UserEmail == email && a.UserPassword == password);
        }



    }
}
