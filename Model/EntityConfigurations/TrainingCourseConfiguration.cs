using Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Model.EntityConfigurations
{
    public class TrainingCourseConfiguration : EntityTypeConfiguration<TrainingCourse>
    {
        public TrainingCourseConfiguration()
        {
            HasMany(tc => tc.EnrolledEmployees)
                .WithMany(e => e.Courses)
                .Map(m =>
                {
                    m.ToTable("CoursesEnrolledEmployees")
                    .MapLeftKey("CourseId")
                    .MapRightKey("EmployeeId");
                });

            HasMany(tc => tc.ReportsFinished)
                .WithMany(r => r.FinishedCourses)
                .Map(m =>
                {
                    m.ToTable("FinishedCoursesReports")
                    .MapLeftKey("CourseId")
                    .MapRightKey("ReportId");
                });

            HasMany(tc => tc.ReportsWished)
                .WithMany(r => r.WishedCourses)
                .Map(m =>
                {
                    m.ToTable("WishedCoursesReports")
                    .MapLeftKey("CourseId")
                    .MapRightKey("ReportId");
                });
        }
    }
}