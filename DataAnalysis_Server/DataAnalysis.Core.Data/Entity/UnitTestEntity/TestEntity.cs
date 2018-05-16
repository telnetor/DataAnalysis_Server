using DataAnalysis.Manipulation.Base;

namespace DataAnalysis.Core.Data.Entity.UnitTestEntity
{
    [Table("Test")]
    public class TestEntity: BaseEntity
    {
        [Field(IsPrimaryKey = true, IsIdentity = true)]
        public int TsId { get; set; }
        public string TsName { get; set; }
        public int TsAge { get; set; }
    }
}
