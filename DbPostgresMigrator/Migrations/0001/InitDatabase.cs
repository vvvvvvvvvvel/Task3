using DbPostgres.Models;
using FluentMigrator;
using Shared.Interfaces.Services;

namespace DbPostgresMigrator.Migrations._0001;

[Migration(1)]
public class InitDatabase : Migration
{
    private readonly IUsersProvider<UserModel> _usersProvider;

    public InitDatabase(IUsersProvider<UserModel> usersProvider)
    {
        _usersProvider = usersProvider;
    }

    private void AddDefaultUsers()
    {
        var users = _usersProvider.Get();
        foreach (var user in users)
            Execute.Sql($@"
            INSERT INTO users 
                VALUES ('{user.Name}', {user.Rating}, {user.Amount});");
    }

    public override void Up()
    {
        Execute.Sql(@"
            CREATE TABLE if not exists users(
                name TEXT PRIMARY KEY,
                rating INT DEFAULT 0,
                amount INT DEFAULT 0
            );");
        AddDefaultUsers();
        Execute.Sql(@"
            CREATE TABLE if not exists coins(
                id BIGSERIAL PRIMARY KEY,
                owner_name TEXT NOT NULL references users(name),
                history TEXT[] DEFAULT '{}',
                history_length INT DEFAULT 0
            );");

        Execute.Sql("CREATE INDEX ON coins (owner_name);");

        Execute.Sql(@"
CREATE FUNCTION ch_coin() RETURNS trigger AS $ch_coin$
    BEGIN
		IF (TG_OP = 'UPDATE') THEN		
			IF NEW.owner_name != OLD.owner_name THEN
				NEW.history = array_append(OLD.history, NEW.owner_name);
				NEW.history_length = array_length(NEW.history, 1);
			END IF;
		ELSEIF (TG_OP = 'INSERT') THEN
			NEW.history = ARRAY[NEW.owner_name];
			NEW.history_length = array_length(NEW.history, 1);
		END IF;
		RETURN NEW;
    END;
$ch_coin$ LANGUAGE plpgsql;");

        Execute.Sql(@"
CREATE TRIGGER ch_coin BEFORE UPDATE OR INSERT ON coins
    FOR EACH ROW EXECUTE PROCEDURE ch_coin();");
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE if exists coins;");
        Execute.Sql("DROP TABLE if exists users;");
        Execute.Sql("DROP FUNCTION if exists ch_coin;");
    }
}