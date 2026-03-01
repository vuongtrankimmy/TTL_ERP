namespace TTL.HR.Application.Modules.Common.Constants
{
    public static class ApiEndpoints
    {
        public const string Version = "";
        public const string AuthPrefix = Version + "auth";
        public const string CorePrefix = Version + "core";

        public static class Auth
        {
            public const string Base = AuthPrefix;
            public const string Login = $"{Base}/login";
            public const string Register = $"{Base}/register";
            public const string Refresh = $"{Base}/refresh";
            public const string Logout = $"{Base}/logout";
            public const string ChangePassword = $"{Base}/change-password";
            public const string ConfirmEmail = $"{Base}/confirm-email";
            public const string RequestPasswordReset = $"{Base}/forgot-password";
            public const string ResetPassword = $"{Base}/reset-password";
        }

        public static class Recruitment
        {
            public const string Base = $"{CorePrefix}/Recruitment";
            public const string Jobs = $"{Base}/jobs";
            public const string Candidates = $"{Base}/candidates";
            public const string CandidatesFull = $"{Base}/candidates";
            public const string Apply = $"{Base}/apply";
        }

        public static class Employees
        {
            public const string Base = $"{CorePrefix}/Employees";
            public const string Dashboard = $"{Base}/dashboard";
            public const string Me = $"{Base}/me";
            public const string AttendanceDetail = $"{Base}/me/attendance";
            public static string DigitalProfile(string employeeId) => $"{Base}/{employeeId}/digital-profile";
            public static string Documents(string employeeId) => $"{Base}/{employeeId}/documents";

        }


        public static class Departments
        {
            public const string Base = $"{CorePrefix}/Departments";
        }

        public static class Positions
        {
            public const string Base = $"{CorePrefix}/Positions";
        }

        public static class Attendance
        {
            public const string Base = $"{CorePrefix}/Attendance";
            public const string Schedule = $"{Base}/schedule";
            public const string Timesheets = $"{Base}/timesheets";
            public const string WorkSchedules = $"{Base}/work-schedules";
            public const string ShiftRequests = $"{Base}/shift-requests";
            public const string Shifts = $"{Base}/shifts";
        }

        public static class Leave
        {
            public const string Base = $"{CorePrefix}/Leave";
        }

        public static class Payroll
        {
            public const string Base = $"{CorePrefix}/Payroll";
            public const string Periods = $"{Base}/periods";
        }

        public static class Training
        {
            public const string Base = $"{CorePrefix}/Training";
            public const string Courses = $"{CorePrefix}/Courses";
            public const string TrainingCourses = $"{Base}/courses";
        }

        public static class Assets
        {
            public const string Base = $"{CorePrefix}/Assets";
        }

        public static class Benefits
        {
            public const string Base = $"{CorePrefix}/Benefits";
        }

        public static class Dashboard
        {
            public const string Base = $"{CorePrefix}/Dashboard/overview";
        }

        public static class Lookups
        {
            public const string Base = $"{CorePrefix}/lookups";
        }

        public static class Contracts
        {
            public const string Base = $"{CorePrefix}/Contracts";
            public const string Templates = $"{CorePrefix}/ContractTemplates";
        }

        public static class Organization
        {
            public const string Structure = $"{CorePrefix}/OrganizationStructure";
            public const string Departments = $"{CorePrefix}/Departments";
            public const string Positions = $"{CorePrefix}/Positions";
        }

        public static class Administration
        {
            public const string Permissions = $"{CorePrefix}/administration/permissions";
            public const string Roles = $"{CorePrefix}/administration/roles";
            public const string PermissionsList = $"{CorePrefix}/administration/roles/permissions";
        }

        public static class System
        {
            public const string Settings = $"{CorePrefix}/system/settings";
            public const string CodeGeneratorConfigs = $"{CorePrefix}/system/settings/code-generator";
            public const string Audit = $"{CorePrefix}/Audit";
        }

    }
}
