using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

/// <summary>
/// Summary description for CompanyWiseAccountClosing
/// </summary>
public class CompanyWiseAccountClosing
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string selectQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    bool datasaved = false;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;

    string DrCr = string.Empty;
    double openingBalance = 0;
    double openingBalanceDebit = 0;
    double openingBalanceCredit = 0;
    double prevDebit = 0;
    double prevCredit = 0;
    double currentDebit = 0;
    double currentCredit = 0;
    double closingBalanceDebit = 0;
    double closingBalanceCredit = 0;
    int ID = 0;

    #endregion [Declarations]

    #region [Constructor]
    public CompanyWiseAccountClosing()
    {
        // TODO: Add constructor logic here
    }
    #endregion [Constructor]

    #region [CompanyWiseDayEndAccountClosingonSave]
    public bool CompanyWiseDayEndAccountClosingonSave(DateTime transDate, int FYID, int compID, int branchID, int accountID, double debit, double credit, SqlTransaction transaction, SqlConnection conn)
    {
        try
        {
            ID = 0;
            datasaved = false;

            //to check whether AccountID is present in " FCompanyDayEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyDayEndClosing where TransDate='" + transDate.ToString("yyyy/MM/dd") + "' and BranchID='" + branchID + "' and  AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = debit;
                currentCredit = credit;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyDayEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyDayEndClosing
                insertQuery = "insert into FCompanyDayEndClosing values(" + ID + ", '" + Convert.ToDateTime(transDate).ToString("yyyy/MM/dd") + "',  " +
                                        "" + FYID + ", " + compID + ", " + branchID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyDayEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = prevDebit + debit;
                currentCredit = prevCredit + credit;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyDayEndClosing
                updateQuery = "update FCompanyDayEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseDayEndAccountClosingonSave]

    #region [CompanyWiseMonthEndAccountClosingonSave]
    public bool CompanyWiseMonthEndAccountClosingonSave(DateTime transDate, int FYID, int compID, int branchID, int accountID, double debit, double credit, SqlTransaction transaction, SqlConnection conn)
    {
        try
        {
            ID = 0;
            datasaved = false;

            //to check whether AccountID is present in " FCompanyMonthEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyMonthEndClosing where (DATEPART(MM, TransDate)=DATEPART(MM, '" + transDate.ToString("yyyy/MM/dd") + "') and DATEPART(YY, TransDate)=DATEPART(YY, '" + transDate.ToString("yyyy/MM/dd") + "')) and BranchID='" + branchID + "' and  AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = debit;
                currentCredit = credit;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyMonthEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyMonthEndClosing
                insertQuery = "insert into FCompanyMonthEndClosing values(" + ID + ", '" + Convert.ToDateTime(transDate).ToString("yyyy/MM/dd") + "',  " +
                                        "" + FYID + ", " + compID + ", " + branchID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyMonthEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = prevDebit + debit;
                currentCredit = prevCredit + credit;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyMonthEndClosing
                updateQuery = "update FCompanyMonthEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseMonthEndAccountClosingonSave]

    #region [CompanyWiseYearEndAccountClosingonSave]
    public bool CompanyWiseYearEndAccountClosingonSave(int FYID, int compID, int branchID, int accountID, double debit, double credit, SqlTransaction transaction, SqlConnection conn)
    {
        datasaved = false;
        try
        {
            ID = 0;

            //to check whether AccountID is present in " FCompanyYearEndClosing table " for the selected financial year.
            selectQuery = "select ID from FCompanyYearEndClosing where FinancialyearID='" + FYID + "' and  AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = debit;
                currentCredit = credit;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyYearEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyYearEndClosing
                insertQuery = "insert into FCompanyYearEndClosing values(" + ID + ", " +
                                        "" + FYID + ", " + compID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyYearEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = prevDebit + debit;
                currentCredit = prevCredit + credit;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyYearEndClosing
                updateQuery = "update FCompanyYearEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            datasaved = false;
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseYearEndAccountClosingonSave]

    #region [CompanyWiseDayEndAccountClosingonEdit]
    public bool CompanyWiseDayEndAccountClosingonEdit(DateTime transDate, int FYID, int compID, int branchID, int accountID, double debit, double credit, double newDebitAmount, double newCreditAmount, SqlTransaction transaction, SqlConnection conn)
    {
        try
        {
            ID = 0;
            datasaved = false;

            //to check whether AccountID is present in " FCompanyDayEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyDayEndClosing where TransDate='" + transDate.ToString("yyyy/MM/dd") + "' and BranchID='" + branchID + "' and  AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = newDebitAmount;
                currentCredit = newCreditAmount;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyDayEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyDayEndClosing
                insertQuery = "insert into FCompanyDayEndClosing values(" + ID + ", '" + Convert.ToDateTime(transDate).ToString("yyyy/MM/dd") + "',  " +
                                        "" + FYID + ", " + compID + ", " + branchID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyDayEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = (prevDebit-debit) + newDebitAmount;
                currentCredit = (prevCredit-credit) + newCreditAmount;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyDayEndClosing
                updateQuery = "update FCompanyDayEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseDayEndAccountClosingonEdit]

    #region [CompanyWiseMonthEndAccountClosingonEdit]
    public bool CompanyWiseMonthEndAccountClosingonEdit(DateTime transDate, int FYID, int compID, int branchID, int accountID, double debit, double credit, double newDebitAmount, double newCreditAmount, SqlTransaction transaction, SqlConnection conn)
    {
        try
        {
            ID = 0;
            datasaved = false;

            //to check whether AccountID is present in " FCompanyMonthEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyMonthEndClosing where (DATEPART(MM, TransDate) = DATEPART(MM, '" + transDate.ToString("yyyy/MM/dd") + "') and DATEPART(YY, TransDate)=DATEPART(YY, '" + transDate.ToString("yyyy/MM/dd") + "')) and BranchID='" + branchID + "' and  AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = newDebitAmount;
                currentCredit = newCreditAmount;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyMonthEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyMonthEndClosing
                insertQuery = "insert into FCompanyMonthEndClosing values(" + ID + ", '" + Convert.ToDateTime(transDate).ToString("yyyy/MM/dd") + "',  " +
                                        "" + FYID + ", " + compID + ", " + branchID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyMonthEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = (prevDebit - debit) + newDebitAmount;
                currentCredit = (prevCredit - credit) + newCreditAmount;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyMonthEndClosing
                updateQuery = "update FCompanyMonthEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseMonthEndAccountClosingonEdit]

    #region [CompanyWiseYearEndAccountClosingonEdit]
    public bool CompanyWiseYearEndAccountClosingonEdit(int FYID, int compID, int branchID, int accountID, double debit, double credit, double newDebitAmount, double newCreditAmount, SqlTransaction transaction, SqlConnection conn)
    {
        datasaved = false;
        try
        {
            ID = 0;

            //to check whether AccountID is present in " FCompanyYearEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyYearEndClosing where FinancialyearID='" + FYID + "' and AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = newDebitAmount;
                currentCredit = newCreditAmount;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyYearEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyYearEndClosing
                insertQuery = "insert into FCompanyYearEndClosing values(" + ID + ",  " +
                                        "" + FYID + ", " + compID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyYearEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = (prevDebit - debit) + newDebitAmount;
                currentCredit = (prevCredit - credit) + newCreditAmount;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyYearEndClosing
                updateQuery = "update FCompanyYearEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            datasaved = false;
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseYearEndAccountClosingonEdit]

    #region [CompanyWiseDayEndAccountClosingonDelete]
    public bool CompanyWiseDayEndAccountClosingonDelete(DateTime transDate, int FYID, int compID, int branchID, int accountID, double debit, double credit, SqlTransaction transaction, SqlConnection conn)
    {
        try
        {
            ID = 0;
            datasaved = false;

            //to check whether AccountID is present in " FCompanyDayEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyDayEndClosing where TransDate='" + transDate.ToString("yyyy/MM/dd") + "' and BranchID='" + branchID + "' and  AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = debit;
                currentCredit = credit;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + (-currentDebit)) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + (-currentCredit)) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyDayEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyDayEndClosing
                insertQuery = "insert into FCompanyDayEndClosing values(" + ID + ", '" + Convert.ToDateTime(transDate).ToString("yyyy/MM/dd") + "',  " +
                                        "" + FYID + ", " + compID + ", " + branchID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyDayEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = prevDebit - debit;
                currentCredit = prevCredit - credit;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyDayEndClosing
                updateQuery = "update FCompanyDayEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseDayEndAccountClosingonDelete]

    #region [CompanyWiseMonthEndAccountClosingonDelete]
    public bool CompanyWiseMonthEndAccountClosingonDelete(DateTime transDate, int FYID, int compID, int branchID, int accountID, double debit, double credit, SqlTransaction transaction, SqlConnection conn)
    {
        try
        {
            ID = 0;
            datasaved = false;

            //to check whether AccountID is present in " FCompanyMonthEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyMonthEndClosing where (DATEPART(MM, TransDate) = DATEPART(MM, '" + transDate.ToString("yyyy/MM/dd") + "') and DATEPART(YY, TransDate)=DATEPART(YY, '" + transDate.ToString("yyyy/MM/dd") + "')) and BranchID='" + branchID + "' and  AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = debit;
                currentCredit = credit;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + (-currentDebit)) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + (-currentCredit)) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyMonthEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyMonthEndClosing
                insertQuery = "insert into FCompanyMonthEndClosing values(" + ID + ", '" + Convert.ToDateTime(transDate).ToString("yyyy/MM/dd") + "',  " +
                                        "" + FYID + ", " + compID + ", " + branchID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyMonthEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = prevDebit - debit;
                currentCredit = prevCredit - credit;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyMonthEndClosing
                updateQuery = "update FCompanyMonthEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseMonthEndAccountClosingonDelete]

    #region [CompanyWiseYearEndAccountClosingonDelete]
    public bool CompanyWiseYearEndAccountClosingonDelete(int FYID, int compID, int branchID, int accountID, double debit, double credit, SqlTransaction transaction, SqlConnection conn)
    {
        datasaved = false;
        try
        {
            ID = 0;

            //to check whether AccountID is present in " FCompanyYearEndClosing table " for the selected date/ financial year.
            selectQuery = "select ID from FCompanyYearEndClosing where FinancialyearID='" + FYID + "' and AccountID='" + accountID + "' ";
            cmd = new SqlCommand(selectQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            #region [if Account ID does not exist]
            if (ID == 0)    //if Account ID does not exist
            {
                openingBalanceDebit = 0;
                openingBalanceCredit = 0;
                closingBalanceDebit = 0;
                closingBalanceCredit = 0;

                currentDebit = debit;
                currentCredit = credit;

                if (currentDebit > 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + (-currentDebit)) - currentCredit);

                    //if Closing Balance Debit is negative it will go to Credit side
                    if (closingBalanceDebit < 0)
                    {
                        string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                        closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                        closingBalanceDebit = 0;
                    }
                }
                else if (currentCredit > 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + (-currentCredit)) - currentDebit);

                    //if Closing Balance Credit is negative it will go to Debit side
                    if (closingBalanceCredit < 0)
                    {
                        string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                        closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                        closingBalanceCredit = 0;
                    }
                }

                //getting MAX ID
                selectQuery = "select max(ID) from FCompanyYearEndClosing";
                cmd = new SqlCommand(selectQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    ID = 0;
                }

                ID += 1;

                //inserting data into table FCompanyYearEndClosing
                insertQuery = "insert into FCompanyYearEndClosing values(" + ID + ", " +
                                        "" + FYID + ", " + compID + ", " +
                                        "" + accountID + ", " + openingBalanceDebit + ", " + openingBalanceCredit + ", " +
                                        "" + currentDebit + ", " + currentCredit + ", " + closingBalanceDebit + ",  " +
                                        "" + closingBalanceCredit + ")";

                cmd = new SqlCommand(insertQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID does not exist]

            #region [if Account ID exists]
            else    //if Account ID exists
            {
                //retrieving OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit
                selectQuery = "select OpeningBalanceDebit, OpeningBalanceCredit, CurrentDebit, CurrentCredit " +
                                "from FCompanyYearEndClosing " +
                                "where ID=" + ID + " ";

                cmd = new SqlCommand(selectQuery, conn, transaction);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        openingBalanceDebit = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        openingBalanceCredit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                        prevDebit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                        prevCredit = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                    }
                    else
                    {
                        openingBalanceDebit = 0;
                        openingBalanceCredit = 0;
                        prevDebit = 0;
                        prevCredit = 0;
                    }
                }
                else
                {
                    openingBalanceDebit = 0;
                    openingBalanceCredit = 0;
                    prevDebit = 0;
                    prevCredit = 0;
                }

                closingBalanceDebit = 0;
                closingBalanceCredit = 0;
                currentDebit = prevDebit - debit;
                currentCredit = prevCredit - credit;

                if (openingBalanceDebit > 0 && openingBalanceCredit == 0)
                {
                    closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                }
                else if (openingBalanceCredit > 0 && openingBalanceDebit == 0)
                {
                    closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                }
                else if (openingBalanceDebit == 0 && openingBalanceCredit == 0)
                {
                    if (currentDebit > 0 && currentDebit > currentCredit)
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                    else if (currentCredit > 0 && currentCredit > currentDebit)
                    {
                        closingBalanceCredit = ((openingBalanceCredit + currentCredit) - currentDebit);
                    }
                    else if ((currentDebit == 0 && currentCredit == 0) || (currentDebit == currentCredit))
                    {
                        closingBalanceDebit = ((openingBalanceDebit + currentDebit) - currentCredit);
                    }
                }

                //if Closing Balance Debit is negative it will go to Credit side
                if (closingBalanceDebit < 0)
                {
                    string strClosingBalanceDebit = Convert.ToString(closingBalanceDebit);
                    closingBalanceCredit = Convert.ToDouble(strClosingBalanceDebit.Replace("-", ""));
                    closingBalanceDebit = 0;
                }

                //if Closing Balance Credit is negative it will go to Debit side
                if (closingBalanceCredit < 0)
                {
                    string strclosingBalanceCredit = Convert.ToString(closingBalanceCredit);
                    closingBalanceDebit = Convert.ToDouble(strclosingBalanceCredit.Replace("-", ""));
                    closingBalanceCredit = 0;
                }

                //updating table FCompanyYearEndClosing
                updateQuery = "update FCompanyYearEndClosing set " +
                                        "OpeningBalanceDebit=" + openingBalanceDebit + ", " +
                                        "OpeningBalanceCredit=" + openingBalanceCredit + ", " +
                                        "CurrentDebit=" + currentDebit + ", CurrentCredit=" + currentCredit + ", " +
                                        "ClosingBalanceDebit=" + closingBalanceDebit + ",  " +
                                        "ClosingBalanceCredit=" + closingBalanceCredit + " " +
                               "where ID=" + ID + "";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            #endregion [if Account ID exists]
        }
        catch (Exception ex)
        {
            datasaved = false;
            throw ex;
        }
        finally
        {
        }
        return datasaved;
    }
    #endregion [CompanyWiseYearEndAccountClosingonDelete]
}