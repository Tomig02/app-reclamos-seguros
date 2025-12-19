namespace app_reclamos_seguros.Model
{
    public interface IClaimsRepository
    {
        // claim
        public abstract void SetArchived(int claimNum, bool isArchived);
        public abstract void SetNewClaim(VehicleClaim claimData);
        public abstract string GetClaimsList(bool wantsArchived);
        public abstract string GetByID(int claimNum);

        // entries
        public abstract string GetAllReportsByID(int claimNumber);
        public abstract void SetNewReport(ClaimReportEntry newReport);
    }
}
