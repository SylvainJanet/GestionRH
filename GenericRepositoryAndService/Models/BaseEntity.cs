namespace GenericRepositoryAndService.Models
{
    /// <summary>
    /// Every classes with an Id have to derive from this class to be
    /// properly handled by the generic repository and service
    /// <br/>
    /// Furthermore,
    /// <list type="bullet">
    /// <item>
    /// For a class t with name "TName", the corresponding repository has to be named "TNameRepository"
    /// </item>
    /// <item>
    /// For a class t with name "TName", the corresponding service has to be named "TNameService"
    /// </item>
    /// </list>
    /// </summary>
    public abstract class BaseEntity
    {
        public int? Id { get; set; }

        public override bool Equals(object obj)
        {
            return obj is BaseEntity entity &&
                   Id == entity.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }
}