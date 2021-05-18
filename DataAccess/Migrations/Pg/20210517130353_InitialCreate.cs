using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataAccess.Migrations.Pg
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageTemplate = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true),
                    TimeStamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Alias = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DashboardKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    MobilePhones = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecordDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UpdateContactDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ResetPasswordExpires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Translates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LangId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translates_Id_LangId",
                        column: x => x.LangId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupClaims",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ClaimId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupClaims", x => new { x.GroupId, x.ClaimId });
                    table.ForeignKey(
                        name: "FK_GroupClaims_Id_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "OperationClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupClaims_Id_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProjectId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "fk_Clients_Users",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    UsersId = table.Column<int>(type: "integer", nullable: false),
                    ClaimId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => new { x.ClaimId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserClaims_Id_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "OperationClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserClaims_Id_UserId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroups_Id_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGroups_Id_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientClaims",
                columns: table => new
                {
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientClaims", x => new { x.ClaimId, x.ClientId });
                    table.ForeignKey(
                        name: "fk_ClientClaims_Client",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ClientGroups_Claim",
                        column: x => x.ClaimId,
                        principalTable: "OperationClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientGroups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGroups", x => new { x.ClientId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroups_Id_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGroups_Id_UserId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "tr-TR", "Türkçe" },
                    { 2, "en-US", "English" }
                });

            migrationBuilder.InsertData(
                table: "Translates",
                columns: new[] { "Id", "Code", "LangId", "Value" },
                values: new object[,]
                {
                    { 1, "Login", 1, "Giriş" },
                    { 64, "PasswordLength", 2, "Must be at least 8 characters long! " },
                    { 62, "PasswordEmpty", 2, "Password can not be empty!" },
                    { 60, "CID", 2, "Citizenship Number" },
                    { 58, "WrongCID", 2, "Citizenship Number Not Found In Our System. Please Create New Registration!" },
                    { 56, "NameAlreadyExist", 2, "The Object You Are Trying To Create Already Exists." },
                    { 54, "SendMobileCode", 2, "Please Enter The Code Sent To You By SMS!" },
                    { 52, "SuccessfulLogin", 2, "Login to the system is successful." },
                    { 50, "PasswordError", 2, "Credentials Failed to Authenticate, Username and / or password incorrect." },
                    { 48, "UserNotFound", 2, "Credentials Could Not Verify. Please use the New Registration Screen." },
                    { 46, "AuthorizationsDenied", 2, "It has been detected that you are trying to enter an area that you do not have authorization." },
                    { 44, "VerifyCid", 2, "Verify Citizen Id" },
                    { 42, "CouldNotBeVerifyCid", 2, "Could not be verify Citizen Id" },
                    { 40, "StringLengthMustBeGreaterThanThree", 2, "Please Enter A Phrase Of At Least 3 Characters." },
                    { 38, "OperationClaimExists", 2, "This operation permit already exists." },
                    { 66, "PasswordUppercaseLetter", 2, "Must Contain At Least 1 Capital Letter!" },
                    { 36, "Deleted", 2, "Successfully Deleted." },
                    { 32, "Added", 2, "Successfully Added." },
                    { 30, "AppMenu", 2, "Application" },
                    { 28, "Management", 2, "Management" },
                    { 26, "TranslateWords", 2, "Translate Words" },
                    { 24, "Languages", 2, "Languages" },
                    { 22, "OperationClaim", 2, "Operation Claim" },
                    { 20, "Groups", 2, "Groups" },
                    { 19, "Users", 2, "Users" },
                    { 18, "Create", 2, "Create" },
                    { 17, "UsersClaims", 2, "User's Claims" },
                    { 16, "UsersGroups", 2, "User's Groups" },
                    { 15, "Delete", 2, "Delete" },
                    { 14, "Update", 2, "Update" },
                    { 13, "Password", 2, "Password" },
                    { 34, "Updated", 2, "Successfully Updated." },
                    { 68, "PasswordLowercaseLetter", 2, "Must Contain At Least 1 Lowercase Letter!" },
                    { 70, "PasswordDigit", 2, "It Must Contain At Least 1 Digit!" },
                    { 72, "PasswordSpecialCharacter", 2, "Must Contain At Least 1 Symbol!" },
                    { 134, "TranslateList", 2, "Translate List" },
                    { 132, "LanguageList", 2, "Language List" },
                    { 130, "OperationClaimList", 2, "OperationClaim List" },
                    { 128, "UserList", 2, "User List" },
                    { 126, "Add", 2, "Add" },
                    { 124, "GrupPermissions", 2, "Grup Permissions" },
                    { 122, "GroupList", 2, "Group List" },
                    { 120, "Permissions", 2, "İzinler" },
                    { 118, "Required", 2, "This field is required!" },
                    { 116, "NoRecordsFound", 2, "No Records Found" },
                    { 114, "MobilePhones", 2, "Mobile Phone" },
                    { 112, "Name", 2, "Name" },
                    { 110, "LangCode", 2, "Lang Code" },
                    { 108, "Value", 2, "Value" },
                    { 106, "Description", 2, "Description" },
                    { 104, "Alias", 2, "Alias" },
                    { 102, "Code", 2, "Code" },
                    { 74, "SendPassword", 2, "Your new password has been sent to your e-mail address." },
                    { 76, "InvalidCode", 2, "You Entered An Invalid Code!" },
                    { 78, "SmsServiceNotFound", 2, "Unable to Reach SMS Service." },
                    { 80, "TrueButCellPhone", 2, "The information is correct. Cell phone is required." },
                    { 82, "TokenProviderException", 2, "Token Provider cannot be empty!" },
                    { 84, "Unknown", 2, "Unknown!" },
                    { 12, "Email", 2, "Email" },
                    { 86, "NewPassword", 2, "New Password:" },
                    { 90, "Save", 2, "Save" },
                    { 92, "GroupName", 2, "Group Name" },
                    { 94, "FullName", 2, "Full Name" },
                    { 96, "Address", 2, "Address" },
                    { 98, "Notes", 2, "Notes" },
                    { 100, "ConfirmPassword", 2, "Confirm Password" },
                    { 88, "ChangePassword", 2, "Change Password" },
                    { 11, "Login", 2, "Login" },
                    { 137, "DeleteConfirm", 1, "Emin misiniz?" },
                    { 135, "LogList", 1, "İşlem Kütüğü" },
                    { 63, "PasswordLength", 1, "Minimum 8 Karakter Uzunluğunda Olmalıdır!" },
                    { 61, "PasswordEmpty", 1, "Parola boş olamaz!" },
                    { 59, "CID", 1, "Vatandaşlık No" },
                    { 57, "WrongCID", 1, "Vatandaşlık No Sistemimizde Bulunamadı. Lütfen Yeni Kayıt Oluşturun!" },
                    { 55, "NameAlreadyExist", 1, "Oluşturmaya Çalıştığınız Nesne Zaten Var." },
                    { 53, "SendMobileCode", 1, "Lütfen Size SMS Olarak Gönderilen Kodu Girin!" },
                    { 51, "SuccessfulLogin", 1, "Sisteme giriş başarılı." },
                    { 49, "PasswordError", 1, "Kimlik Bilgileri Doğrulanamadı, Kullanıcı adı ve/veya parola hatalı." },
                    { 47, "UserNotFound", 1, "Kimlik Bilgileri Doğrulanamadı. Lütfen Yeni Kayıt Ekranını kullanın." },
                    { 45, "AuthorizationsDenied", 1, "Yetkiniz olmayan bir alana girmeye çalıştığınız tespit edildi." },
                    { 43, "VerifyCid", 1, "Kimlik No Doğrulandı." },
                    { 41, "CouldNotBeVerifyCid", 1, "Kimlik No Doğrulanamadı." },
                    { 39, "StringLengthMustBeGreaterThanThree", 1, "Lütfen En Az 3 Karakterden Oluşan Bir İfade Girin." },
                    { 37, "OperationClaimExists", 1, "Bu operasyon izni zaten mevcut." },
                    { 35, "Deleted", 1, "Başarıyla Silindi." },
                    { 33, "Updated", 1, "Başarıyla Güncellendi." },
                    { 31, "Added", 1, "Başarıyla Eklendi." },
                    { 2, "Email", 1, "E posta" },
                    { 3, "Password", 1, "Parola" },
                    { 4, "Update", 1, "Güncelle" },
                    { 5, "Delete", 1, "Sil" },
                    { 6, "UsersGroups", 1, "Kullanıcının Grupları" },
                    { 7, "UsersClaims", 1, "Kullanıcının Yetkileri" },
                    { 65, "PasswordUppercaseLetter", 1, "En Az 1 Büyük Harf İçermelidir!" },
                    { 8, "Create", 1, "Yeni" },
                    { 10, "Groups", 1, "Gruplar" },
                    { 21, "OperationClaim", 1, "Operasyon Yetkileri" },
                    { 23, "Languages", 1, "Diller" },
                    { 25, "TranslateWords", 1, "Dil Çevirileri" },
                    { 27, "Management", 1, "Yönetim" },
                    { 29, "AppMenu", 1, "Uygulama" },
                    { 9, "Users", 1, "Kullanıcılar" },
                    { 136, "LogList", 2, "LogList" },
                    { 67, "PasswordLowercaseLetter", 1, "En Az 1 Küçük Harf İçermelidir!" },
                    { 71, "PasswordSpecialCharacter", 1, "En Az 1 Simge İçermelidir!" },
                    { 133, "TranslateList", 1, "Dil Çeviri Listesi" },
                    { 131, "LanguageList", 1, "Dil Listesi" },
                    { 129, "OperationClaimList", 1, "Yetki Listesi" },
                    { 127, "UserList", 1, "Kullanıcı Listesi" },
                    { 125, "Add", 1, "Ekle" },
                    { 123, "GrupPermissions", 1, "Grup Yetkileri" },
                    { 121, "GroupList", 1, "Grup Listesi" },
                    { 119, "Permissions", 1, "Permissions" },
                    { 117, "Required", 1, "Bu alan zorunludur!" },
                    { 115, "NoRecordsFound", 1, "Kayıt Bulunamadı" },
                    { 113, "MobilePhones", 1, "Cep Telefonu" },
                    { 111, "Name", 1, "Adı" },
                    { 109, "LangCode", 1, "Dil Kodu" },
                    { 107, "Value", 1, "Değer" },
                    { 105, "Description", 1, "Açıklama" },
                    { 103, "Alias", 1, "Görünen Ad" },
                    { 101, "Code", 1, "Kod" },
                    { 73, "SendPassword", 1, "Yeni Parolanız E-Posta Adresinize Gönderildi." },
                    { 75, "InvalidCode", 1, "Geçersiz Bir Kod Girdiniz!" },
                    { 77, "SmsServiceNotFound", 1, "SMS Servisine Ulaşılamıyor." },
                    { 79, "TrueButCellPhone", 1, "Bilgiler doğru. Cep telefonu gerekiyor." },
                    { 81, "TokenProviderException", 1, "Token Provider boş olamaz!" },
                    { 83, "Unknown", 1, "Bilinmiyor!" },
                    { 69, "PasswordDigit", 1, "En Az 1 Rakam İçermelidir!" },
                    { 85, "NewPassword", 1, "Yeni Parola:" },
                    { 89, "Save", 1, "Kaydet" },
                    { 91, "GroupName", 1, "Grup Adı" },
                    { 93, "FullName", 1, "Tam Adı" },
                    { 95, "Address", 1, "Adres" },
                    { 97, "Notes", 1, "Notlar" },
                    { 99, "ConfirmPassword", 1, "Parolayı Doğrula" },
                    { 87, "ChangePassword", 1, "Parola Değiştir" },
                    { 138, "DeleteConfirm", 2, "Are you sure?" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaims",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGroups_GroupId",
                table: "ClientGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CustomerId",
                table: "Clients",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupClaims_ClaimId",
                table: "GroupClaims",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Translates_LangId",
                table: "Translates",
                column: "LangId");

            migrationBuilder.CreateIndex(
                name: "Uk_Uniqe",
                table: "UserClaims",
                columns: new[] { "UsersId", "ClaimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientClaims");

            migrationBuilder.DropTable(
                name: "ClientGroups");

            migrationBuilder.DropTable(
                name: "GroupClaims");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Translates");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "OperationClaims");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
