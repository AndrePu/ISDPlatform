using Cooper.DAO.Mapping;
using Cooper.DAO.Models;
using Cooper.ORM;
using Cooper.Services.Interfaces;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooper.DAO
{
    public class UserDAO : IUserDAO
    {
        private readonly DbConnect dbConnect;
        private readonly OracleConnection Connection;
        private readonly Logger logger;
        private readonly CRUD crud;

        private string table;
        private string idColumn;
        private HashSet<string> attributes;
        private HashSet<string> unique_attributes;

        public UserDAO(IConfigProvider configProvider)
        {
            crud = new CRUD(configProvider);
            dbConnect = new DbConnect(configProvider);
            Connection = dbConnect.GetConnection();
            logger = LogManager.GetLogger("CooperLoger");

            table = "USERS";
            idColumn = "ID";

            attributes = new HashSet<string>()
            {
                "ID", "NAME", "NICKNAME", "EMAIL", "PASSWORD", "PHOTOURL",
                "ISVERIFIED", "ISCREATOR", "ISBANNED", "ENDBANDATE", "DESCRIPTION",
                "PLATFORMLANGUAGE", "PLATFORMTHEME"
            };

            unique_attributes = new HashSet<string>()
            {
                "ID", "NICKNAME", "EMAIL"
            };
        }

        #region Checking methods

        public bool IfNicknameExists(string nickname)
        {
            UserDb user = GetByNickname(nickname);

            return (user != null);
        }

        public bool IfEmailExists(string email)
        {
            UserDb user = GetByEmail(email);

            return (user != null);
        }

        public bool IfPasswordCorrect(long id, string password)
        {
            UserDb user = Get(id);

            return user.Password == password;
        }

        public bool IfUserExists(long id)
        {
            UserDb user = Get(id);

            return (user != null);
        }

        public bool CheckCredentials(string nickname, string password)
        {
            UserDb user = GetByNickname(nickname);

            if (user == null || user.Password != password)
            {
                user = GetByEmail(nickname);

                if (user == null || user.Password != password)
                {
                    return false;
                }
            }
            
            return true;
        }

        #endregion

        #region Main methods

        #region Get methods

        public UserDb Get(object id)
        {
            string attribute = "ID";

            return GetByUniqueAttribute(id, attribute);
        }

        public UserDb GetByNickname(string nickname)
        {
            string attribute = "NICKNAME";

            nickname = $"\'{nickname}\'";       // tuning string for sql query
            
            return GetByUniqueAttribute(nickname, attribute);
        }

        public UserDb GetByEmail(string email)
        {
            string attribute = "EMAIL";
            
            email = $"\'{email}\'";       // tuning string for sql query

            return GetByUniqueAttribute(email, attribute);
        }

        public UserDb GetByUniqueAttribute(object attribute_value, string attribute_name)
        {
            UserDb user = null;

            if (!unique_attributes.Contains(attribute_name))
            {
                logger.Info($"{attribute_name} is not unique attribute. GET-method failed..");
                return user;
            }

            List<EntityORM> entities = (List<EntityORM>)(crud.Read(table, attributes, new DbTools.WhereRequest[] { new DbTools.WhereRequest(attribute_name, DbTools.RequestOperator.Equal, attribute_value) }));

            if (entities.Any()) {
                EntityMapping.Map(entities[0], out user);
            }

            return user;
        }

        public UserDb GetExtended(long id)
        {
            UserDb user = Get(id);

            if (user != null)
            {
                // TODO: remake userConnections system for user model
                //user.ConnectionsList = (List<long>)userConnectionDAO.GetConnectionsListByUserId(user.Id);
            }

            return user;
        }

        public IEnumerable<UserDb> GetAll()
        {
            List<UserDb> users = new List<UserDb>();

            List<EntityORM> entities = (List<EntityORM>)crud.Read(table, attributes);

            foreach (EntityORM entity in entities)              // Mapping entities to users
            {
                EntityMapping.Map(entity, out UserDb user);
                users.Add(user);
            }

            return users;
        }
        
        #region Interop properties info reading
        private List<long> GetConnectionsList(long id)
        {
            List<long> connectionList = new List<long>();

            string sqlExpression = $"SELECT ID from USERSCONNECTIONS WHERE user1 = {id}";

            try
            {
                Connection.Open();

                OracleCommand command = new OracleCommand(sqlExpression, Connection);
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    long idUser = Convert.ToInt64(reader["user1"]);
                    connectionList.Add(idUser);
                }

            }
            catch (Exception ex)
            {
                logger.Info("Exception.Message: {0}", ex.Message);
            }
            finally
            {
                Connection.Close();
            }



            return connectionList;
        }


        #endregion

        #endregion
            
        public long Save(UserDb user)
        {
            EntityORM entity = EntityMapping.Map(user, attributes);

            // Making sure that ID value is not touched.
            entity.attributeValue.Remove("ID");

            long user_id = crud.Create(table, idColumn, entity);

            logger.Info($"User with id = {user_id} was created");

            return user_id;
        }

        public void Delete(object id)
        {
            bool ifDeleted = crud.Delete(id, table, idColumn);

            if (ifDeleted)
            {
                logger.Info($"User with id={id} was successfully deleted from table {table}.");
            }
            else
            {
                logger.Info($"Deleting user with id={id} was failed.");
            }

        }

        public void Update(UserDb user)
        {
            Update(user, removePassword: false);
        }

        public void Update(UserDb user, bool removePassword)
        {
            EntityORM entity = EntityMapping.Map(user, attributes);

            // Making sure that ID value is not touched.
            entity.attributeValue.Remove("ID");
            if (removePassword) { //Remove password field
                entity.attributeValue.Remove("PASSWORD");
            } 

            bool ifUpdated = crud.Update(user.Id, table, idColumn, entity);

            if (ifUpdated)
            {
                logger.Info($"User with id={user.Id} was successfully updated.");
            }
            else
            {
                logger.Info($"Updating user with id={user.Id} was failed.");
            }
        }

        #endregion
    }
}
