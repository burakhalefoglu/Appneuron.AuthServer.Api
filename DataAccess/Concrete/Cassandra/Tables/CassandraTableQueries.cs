namespace DataAccess.Concrete.Cassandra.Tables
{
    public static class CassandraTableQueries
    {
        public static string UserProject => "CREATE TABLE IF NOT EXISTS authdatabase.user_projects(id bigint, user_id bigint, project_id bigint, status boolean,  PRIMARY KEY(id))";
        public static string Client => "CREATE TABLE IF NOT EXISTS authdatabase.clients(id bigint, project_id bigint, status boolean,  PRIMARY KEY(id))";
        public static string Group => "CREATE TABLE IF NOT EXISTS authdatabase.groups(id bigint, group_name text, status boolean, PRIMARY KEY(id))";
        public static string GroupClaim => "CREATE TABLE IF NOT EXISTS authdatabase.group_claims(id bigint, group_id bigint, claim_id bigint, status boolean, PRIMARY KEY(id))";
        public static string Language => "CREATE TABLE IF NOT EXISTS authdatabase.languages(id bigint, name text, code text, status boolean, PRIMARY KEY(id))";
        public static string Log => "CREATE TABLE IF NOT EXISTS authdatabase.logs(id bigint, message_template text, level text, time_stamp date, exception text, status boolean, PRIMARY KEY(id))";
        public static string OperationClaim => "CREATE TABLE IF NOT EXISTS authdatabase.operation_claims(id bigint, name text, alias text, description text, status boolean, PRIMARY KEY(id))";
        public static string Translate => "CREATE TABLE IF NOT EXISTS authdatabase.translates(id bigint, code text, value text, status boolean, PRIMARY KEY(id))";
        public static string User => "CREATE TABLE IF NOT EXISTS authdatabase.users(id bigint, name text, email text, record_date timestamp, update_contact_date timestamp, password_salt blob, password_hash blob, reset_password_token text, reset_password_expires timestamp, status boolean, PRIMARY KEY((email, status), id))";
        public static string UserClaim => "CREATE TABLE IF NOT EXISTS authdatabase.user_claims(id bigint, users_id bigint, claim_id bigint, status boolean, PRIMARY KEY(id))";
        public static string UserGroup => "CREATE TABLE IF NOT EXISTS authdatabase.user_groups(id bigint, users_id bigint, group_id bigint, status boolean, PRIMARY KEY(id))";
    }
}