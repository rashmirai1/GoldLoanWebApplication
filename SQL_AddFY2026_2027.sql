-- ============================================================
-- Add Financial Year: April 2026 - March 2027
-- Table: tblFinancialyear
-- Date: 2026-04-14
-- ============================================================

-- Step 1: Check existing financial years
SELECT FinancialyearID, StartDate, EndDate, Financialyear, CompID
FROM tblFinancialyear
WHERE CompID = 1
ORDER BY FinancialyearID DESC

-- Step 2: Check if it already exists
SELECT COUNT(*) AS AlreadyExists
FROM tblFinancialyear
WHERE Financialyear = 'April2026-March2027'

-- Step 3: Insert new financial year (only if not exists)
IF NOT EXISTS (SELECT 1 FROM tblFinancialyear WHERE Financialyear = 'April2026-March2027')
BEGIN
    DECLARE @FYID INT
    SELECT @FYID = ISNULL(MAX(FinancialyearID), 0) + 1 FROM tblFinancialyear

    INSERT INTO tblFinancialyear
    VALUES (
        @FYID,
        '2026/04/01',   -- StartDate
        '2027/03/31',   -- EndDate
        '2026/04/01',   -- StartDate (duplicate column)
        '2027/03/31',   -- EndDate (duplicate column)
        'April2026-March2027',  -- Financialyear (display text)
        1               -- CompID
    )

    PRINT 'Financial Year April2026-March2027 added successfully with FYID = ' + CAST(@FYID AS VARCHAR)
END
ELSE
BEGIN
    PRINT 'Financial Year April2026-March2027 already exists.'
END

-- Step 4: Verify the insert
SELECT FinancialyearID, StartDate, EndDate, Financialyear, CompID
FROM tblFinancialyear
WHERE CompID = 1
ORDER BY FinancialyearID DESC
