using Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Model.EntityConfigurations
{
    public class SkillConfiguration : EntityTypeConfiguration<Skill>
    {
        public SkillConfiguration()
        {
            HasMany(s => s.Courses)
                .WithMany(c => c.TrainedSkills)
                .Map(m =>
                {
                    m.ToTable("TraninedSkillsCourses")
                    .MapLeftKey("SkillId")
                    .MapRightKey("TagId");
                });

            HasMany(s => s.Posts)
                .WithMany(p => p.RequiredSkills)
                .Map(m =>
                {
                    m.ToTable("RequiredSkillsPosts")
                    .MapLeftKey("SkillId")
                    .MapRightKey("PostId");
                });

            HasMany(s => s.Employees)
                .WithMany(e => e.Skills)
                .Map(m =>
                {
                    m.ToTable("SkillsEmployees")
                    .MapLeftKey("SkillId")
                    .MapRightKey("EmployeeId");
                });
        }
    }
}