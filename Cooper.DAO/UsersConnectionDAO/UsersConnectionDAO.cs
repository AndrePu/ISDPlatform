﻿using Cooper.DAO.Mapping;
using Cooper.DAO.Models;
using Cooper.Models.UserConnectionsEnumTypes;
using Cooper.ORM;
using Cooper.Services.Interfaces;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Cooper.DAO
{
    public class UsersConnectionDAO : IUsersConnectionDAO
    {
        private readonly Logger logger;
        private readonly ICRUD crud;
        private readonly IUserDAO userDAO;
        private readonly ISession session;

        private string idColumn;
        private string table;
        private HashSet<string> attributes;

        private readonly OracleConnection oracleConnection;

        public UsersConnectionDAO(IConfigProvider configProvider, ISession session)
        {
            crud = new CRUD(configProvider);
            logger = LogManager.GetLogger("CooperLoger");
            userDAO = new UserDAO(configProvider);

            oracleConnection = (OracleConnection)session.GetConnection();

            this.session = session;

            table = "USERSCONNECTIONS";
            idColumn = "ID";

            attributes = new HashSet<string>()
            {
                "ID", "AREFRIENDS", "BLACKLISTED", "IDUSER1", "IDUSER2"
            };
        }

        public List<UserDb> GetSpecifiedTypeUsersList(object userId, ConnectionType connectionType)
        {
            List<UserDb> usersList = null;
            
            WhereRequest whereRequest = null;

            string user1_attribute = String.Empty;
            string user2_attribute = String.Empty;

            switch (connectionType)
            {
                case ConnectionType.Subscribers:
                    {
                        user1_attribute = "IDUSER1";

                        whereRequest = new WhereRequest(user1_attribute, Operators.Equal, userId.ToString())
                            .And("BLACKLISTED", Operators.Equal, DbTools.WrapBoolean(false));

                        user2_attribute = "IDUSER2";

                        break;
                    }
                case ConnectionType.Blacklist:
                    {
                        user1_attribute = "IDUSER1";

                        whereRequest = new WhereRequest(user1_attribute, Operators.Equal, userId.ToString())
                            .And("BLACKLISTED", Operators.Equal, DbTools.WrapBoolean(true));

                        user2_attribute = "IDUSER2";
                        break;
                    }
                case ConnectionType.Subscriptions:
                    {
                        user1_attribute = "IDUSER2";

                        whereRequest = new WhereRequest(user1_attribute, Operators.Equal, userId.ToString())
                            .And("BLACKLISTED", Operators.Equal, DbTools.WrapBoolean(false));

                        user2_attribute = "IDUSER1";
                        break;
                    }
                case ConnectionType.Friends:
                    {
                        user1_attribute = "IDUSER1";

                        whereRequest = new WhereRequest(user1_attribute, Operators.Equal, userId.ToString())
                            .And("AREFRIENDS", Operators.Equal, DbTools.WrapBoolean(true));

                        user2_attribute = "IDUSER2";
                        break;
                    }
                default:
                    break;                    
            }

            try
            {
                oracleConnection.Open();


                List<EntityORM> userConnections = (List<EntityORM>)Read(table, attributes, whereRequest);
                if (userConnections != null)
                {
                    usersList = new List<UserDb>();

                    foreach (var usersConnection in userConnections)
                    {
                        long relatedUserId = Convert.ToInt64(usersConnection.attributeValue[user2_attribute]);
                        UserDb user = userDAO.Get(relatedUserId);

                        usersList.Add(user);
                    }
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                oracleConnection.Close();
            }

            return usersList;
        }

        public bool CreateConnection(UsersConnectionDb usersConnection)
        {
            bool isCreated = true;            

            try
            {

                if (ConnectionExists(usersConnection))
                    return false;

                UsersConnectionDb symmetricConnection = new UsersConnectionDb() { IdUser1 = usersConnection.IdUser2, IdUser2 = usersConnection.IdUser1 };

                var whereRequest = new WhereRequest("IDUSER1", Operators.Equal, symmetricConnection.IdUser1.ToString())
                    .And("IDUSER2", Operators.Equal, symmetricConnection.IdUser2.ToString());

                EntityORM entity = Get(table, attributes, whereRequest);

                if (entity != null)
                {
                    if (DbTools.ProcessBoolean(entity.attributeValue["BLACKLISTED"]))
                    {
                        isCreated = false;

                        return isCreated;
                    }

                    symmetricConnection.AreFriends = true;

                    EntityORM symmetricConnection_newTyped = Mapping.EntityMapping.Map(symmetricConnection, attributes);

                    // Making sure that ID value is not touched.
                    symmetricConnection_newTyped.attributeValue.Remove("ID");

                    Update(table, attributes, whereRequest, symmetricConnection_newTyped);

                    usersConnection.AreFriends = true;
                }

                whereRequest = new WhereRequest("IDUSER1", Operators.Equal, symmetricConnection.IdUser1.ToString())
                    .And("IDUSER2", Operators.Equal, symmetricConnection.IdUser2.ToString());

                EntityORM usersConnection_newTyped = Mapping.EntityMapping.Map(usersConnection, attributes);

                // Making sure that ID value is not touched.
                usersConnection_newTyped.attributeValue.Remove("ID");

                Create(table, usersConnection_newTyped);
            }
            catch (DbException ex)
            {
                logger.Info("Creating subscription user with id={1} on user with id={0} failed: {2}", usersConnection.IdUser1, usersConnection.IdUser2, ex.Message);

                isCreated = false;
            }

            return isCreated;
        }

        private bool ConnectionExists(UsersConnectionDb usersConnection)
        {
            var  whereRequest = new WhereRequest("IDUSER1", Operators.Equal, usersConnection.IdUser1.ToString())
                .And("IDUSER2", Operators.Equal, usersConnection.IdUser2.ToString());

            EntityORM usersConnection_newTyped = Get(table, attributes, whereRequest);

            return usersConnection_newTyped != null;
        }

        public bool BanUser(UsersConnectionDb usersConnection)
        {

            bool isBanned = true;
            bool isUnsubscribed = false;

            try
            {

                UsersConnectionDb symmetricConnection = new UsersConnectionDb() { IdUser1 = usersConnection.IdUser2, IdUser2 = usersConnection.IdUser1 };

                var whereRequest = new WhereRequest("IDUSER1", Operators.Equal, symmetricConnection.IdUser1.ToString())
                    .And("IDUSER2", Operators.Equal, symmetricConnection.IdUser2.ToString());

                if (ConnectionExists(symmetricConnection))
                {
                    isUnsubscribed = true;
                    Delete(table, whereRequest);
                }

                whereRequest = new WhereRequest("IDUSER1", Operators.Equal, usersConnection.IdUser1.ToString())
                    .And("IDUSER2", Operators.Equal, usersConnection.IdUser2.ToString());

                EntityORM usersConnection_newTyped = Mapping.EntityMapping.Map(usersConnection, attributes);

                // Making sure that ID value is not touched.
                usersConnection_newTyped.attributeValue.Remove("ID");

                if (ConnectionExists(usersConnection))
                {
                    if (isUnsubscribed)
                    {
                        usersConnection.AreFriends = false;
                    }

                    Update(table, attributes, whereRequest, usersConnection_newTyped);
                }
                else
                {
                    Create(table, usersConnection_newTyped);
                }

            }
            catch (DbException ex)
            {
                logger.Info("Banning user with id={1} by user with id={0} failed: {2}", usersConnection.IdUser1, usersConnection.IdUser2, ex.Message);
                
                isBanned = false;
            }

            return isBanned;
        }

        public bool UnbanUser(UsersConnectionDb usersConnection)
        {
            bool isUnbanned = true;

            var whereRequest = new WhereRequest("IDUSER1", Operators.Equal, usersConnection.IdUser1.ToString())
                .And("IDUSER2", Operators.Equal, usersConnection.IdUser2.ToString());

            try
            {
                Delete(table, whereRequest);
            }
            catch (DbException ex)
            {
                logger.Info("Unbanning user with id={1} by user with id={0} failed: {2}", usersConnection.IdUser1, usersConnection.IdUser2, ex.Message);
                isUnbanned = false;
            }


            return isUnbanned;
        }

        public bool Unsubscribe(UsersConnectionDb usersConnection)
        {
            bool isUnsubscribed = true;
            
            try
            {

                var whereRequest = new WhereRequest("IDUSER1", Operators.Equal, usersConnection.IdUser1.ToString())
                    .And("IDUSER2", Operators.Equal, usersConnection.IdUser2.ToString());

                Delete(table, whereRequest);

                UsersConnectionDb symmetricConnection = new UsersConnectionDb() { IdUser1 = usersConnection.IdUser2, IdUser2 = usersConnection.IdUser1 };

                if (ConnectionExists(symmetricConnection))
                {
                    whereRequest = new WhereRequest("IDUSER1", Operators.Equal, symmetricConnection.IdUser1.ToString())
                        .And("IDUSER2", Operators.Equal, symmetricConnection.IdUser2.ToString());

                    symmetricConnection.AreFriends = false;

                    EntityORM symmetricConnection_newTyped = Mapping.EntityMapping.Map(symmetricConnection, attributes);

                    // Making sure that ID value is not touched.
                    symmetricConnection_newTyped.attributeValue.Remove("ID");

                    Update(table, attributes, whereRequest, symmetricConnection_newTyped);
                }
                
                logger.Info($"User with id={usersConnection.IdUser1} has succesfully unsubscribed from user with id={usersConnection.IdUser2}");
            }
            catch (DbException ex)
            {
                logger.Info("Unsubcription user with id={0} from user with id={1} failed: {2}", usersConnection.IdUser1, usersConnection.IdUser2, ex.Message);

                isUnsubscribed = false;
            }

            return isUnsubscribed;
        }

        public void Create(string table, EntityORM entity)
        {

            #region Creating SQL expression text
            string sqlExpression = String.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                table,
                String.Join(",", entity.attributeValue.Keys),
                String.Join(",", entity.attributeValue.Values));
            #endregion

            OracleCommand command = new OracleCommand(sqlExpression, (OracleConnection)session.GetConnection());
            command.Transaction = (OracleTransaction)session.GetTransaction();

            command.ExecuteNonQuery();
        }
        
        private EntityORM Get(string table, HashSet<string> attributes, WhereRequest whereRequest)
        {
            EntityORM entity = null;
            
            string sqlExpression = DbTools.CreateSelectQuery(table, attributes, whereRequest);

            OracleCommand command = new OracleCommand(sqlExpression, (OracleConnection)session.GetConnection());
            command.Transaction = (OracleTransaction)session.GetTransaction();

            OracleDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                entity = new EntityORM();

                foreach (string attribute in attributes)
                {
                    object value = reader[attribute];
                    entity.attributeValue.Add(attribute, value);
                }
            }

            return entity;
        }

        private void Delete(string table, WhereRequest whereRequest)
        {
            string sqlExpression = DbTools.CreateDeleteQuery(table, whereRequest);
            

            OracleCommand command = new OracleCommand(sqlExpression, (OracleConnection)session.GetConnection());
            command.Transaction = (OracleTransaction)session.GetTransaction();
            command.ExecuteNonQuery();
        }

        private void Update(string table, HashSet<string> attributes, WhereRequest whereRequest, EntityORM entity)
        {
            string sqlExpression = DbTools.CreateUpdateQuery(table, entity, whereRequest);
            
            var oracleCommand = new OracleCommand(sqlExpression, (OracleConnection)session.GetConnection());
            oracleCommand.Transaction = (OracleTransaction)session.GetTransaction();

            oracleCommand.ExecuteNonQuery();
        }

        public IEnumerable<EntityORM> Read(string table, HashSet<string> attributes, WhereRequest whereRequest = null)
        {
            List<EntityORM> entities = new List<EntityORM>();
            try
            {
                string sqlExpression = DbTools.CreateSelectQuery(table, attributes, whereRequest);
                
                OracleCommand command = new OracleCommand(sqlExpression, oracleConnection);

                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EntityORM entity = new EntityORM();
                    foreach (string attribute in attributes)
                    {
                        entity.attributeValue.Add(DbTools.GetVariableAttribute(attribute), reader[attribute]);
                    }

                    entities.Add(entity);
                }
            }
            catch (DbException ex)
            {
                logger.Info("Exception.Message: {0}", ex.Message);
            }

            return entities;
        }

    }
}
