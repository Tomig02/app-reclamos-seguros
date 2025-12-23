namespace app_reclamos_seguros.Model
{
    public interface IClaimsRepository
    {
        // claim
        public abstract void SetArchived(int claimNum, bool isArchived);
        public abstract void SetNewClaim(VehicleClaim claimData);
        public abstract ClaimSearchResult GetClaimsList(bool wantsArchived);
        public abstract VehicleClaim? GetByID(int claimNum);

        // entries
        public abstract List<ClaimReportEntry> GetAllReportsByID(int claimNumber);
        public abstract void SetNewReport(ClaimReportEntry newReport);
    }
}
