namespace DataAccess.Concrete.Cassandra.Tables
{
    public static class CassandraTableQueries
    {
        public static string UserProject => "CREATE TABLE IF NOT EXISTS authdatabase.user_project(id bigint, user_id bigint, project_id bigint, status boolean,  PRIMARY KEY(id))";
        public static string Client => "CREATE TABLE IF NOT EXISTS authdatabase.client(id bigint, project_id bigint, status boolean,  PRIMARY KEY(id))";
        public static string Group => "CREATE TABLE IF NOT EXISTS authdatabase.group(id bigint, group_name text, status boolean, PRIMARY KEY(id))";
        public static string GroupClaim => "CREATE TABLE IF NOT EXISTS authdatabase.group_claim(id bigint, group_id bigint, claim_id bigint, status boolean, PRIMARY KEY(id))";
        public static string Language => "CREATE TABLE IF NOT EXISTS authdatabase.language(id bigint, name text, code text, status boolean, PRIMARY KEY(id))";
        public static string Log => "CREATE TABLE IF NOT EXISTS authdatabase.log(id bigint, message_template text, level text, time_stamp date, exception text, status boolean, PRIMARY KEY(id))";
        public static string OperationClaim => "CREATE TABLE IF NOT EXISTS authdatabase.operation_claim(id bigint, name text, alias text, description text, status boolean, PRIMARY KEY(id))";
        public static string Translate => "CREATE TABLE IF NOT EXISTS authdatabase.translate(id bigint, code text, value text, status boolean, PRIMARY KEY(id))";
        public static string User => "CREATE TABLE IF NOT EXISTS authdatabase.user(id bigint, name text, email text, record_date date, update_contact_date date, password_salt blob, password_hash blob, reset_password_token text, reset_password_expires text, status boolean, PRIMARY KEY(email))";
        public static string UserClaim => "CREATE TABLE IF NOT EXISTS authdatabase.user_claim(id bigint, users_ıd bigint, claim_id bigint, status boolean, PRIMARY KEY(id))";
        public static string UserGroup => "CREATE TABLE IF NOT EXISTS authdatabase.user_group(id bigint, users_ıd bigint, group_id bigint, status boolean, PRIMARY KEY(id))";
    }
}