/*
 * 002_SeedData.sql
 * Seeds the AASHTOware Project Dashboard with realistic transportation project data.
 * Must be run after 001_CreateTables.sql.
 */

USE [AASHTOware];
GO

-- =============================================================================
-- Projects
-- =============================================================================
SET IDENTITY_INSERT [dbo].[Projects] ON;

INSERT INTO [dbo].[Projects]
    ([ProjectId], [Name], [Status], [StartDate], [EndDate], [TotalBudget], [Location], [CreatedDate], [ModifiedDate])
VALUES
    (1, N'I-75 Corridor Resurfacing - Phase II',           N'Active',    '2025-03-15', NULL,         18500000.00, N'I-75 from MP 42.3 to MP 67.8, Hamilton County, OH',          '2025-01-10T08:00:00', '2025-09-22T14:35:00'),
    (2, N'US-30 Bridge Replacement over Cedar Creek',      N'Completed', '2023-06-01', '2025-01-20', 6750000.00,  N'US-30 at Cedar Creek crossing, Allen County, IN',            '2023-04-15T10:00:00', '2025-01-20T16:00:00'),
    (3, N'SR-9 Safety Improvement & Guardrail Upgrade',    N'OnHold',    '2025-07-01', NULL,         4200000.00,  N'SR-9 from MP 12.0 to MP 19.5, Monroe County, WV',            '2025-05-20T09:30:00', '2025-08-10T11:15:00'),
    (4, N'Downtown Multimodal Transit Hub Construction',   N'Draft',     '2026-01-15', NULL,         32000000.00, N'Block 400-402, Main St & 3rd Ave, Lexington, KY',            '2025-11-01T13:00:00', '2025-12-18T09:45:00'),
    (5, N'Route 119 Drainage Rehabilitation',              N'Cancelled', '2024-09-01', '2025-03-01', 2100000.00,  N'Route 119 from MP 5.0 to MP 8.2, Fayette County, PA',        '2024-07-10T07:45:00', '2025-03-01T10:00:00');

SET IDENTITY_INSERT [dbo].[Projects] OFF;
GO

-- =============================================================================
-- ProjectItems  (35+ bid items using AASHTO standard pay-item codes)
-- =============================================================================
SET IDENTITY_INSERT [dbo].[ProjectItems] ON;

INSERT INTO [dbo].[ProjectItems]
    ([ProjectItemId], [ProjectId], [ItemNumber], [Description], [Quantity], [Unit], [UnitPrice], [Amount], [CreatedDate])
VALUES
    -- Project 1: I-75 Corridor Resurfacing
    ( 1, 1, N'110-01',  N'Mobilization',                            1.0000, N'LS',   850000.0000,  850000.00,  '2025-01-10T08:00:00'),
    ( 2, 1, N'201-01',  N'Clearing and Grubbing',                   15.0000, N'ACRE', 3200.0000,    48000.00,   '2025-01-10T08:00:00'),
    ( 3, 1, N'301-01',  N'Aggregate Base Course, Type A',           24500.0000, N'TON', 28.5000,     698250.00,  '2025-01-10T08:00:00'),
    ( 4, 1, N'403-01',  N'Hot Mix Asphalt Surface Course, PG 76-22', 42000.0000, N'TON', 95.0000,   3990000.00, '2025-01-10T08:00:00'),
    ( 5, 1, N'403-02',  N'Hot Mix Asphalt Intermediate Course',     38000.0000, N'TON', 82.0000,    3116000.00, '2025-01-10T08:00:00'),
    ( 6, 1, N'403-05',  N'Milling Existing Asphalt Pavement',       185000.0000, N'SY', 4.7500,     878750.00,  '2025-01-10T08:00:00'),
    ( 7, 1, N'606-01',  N'Guardrail, W-Beam, Type 1',              12400.0000, N'LF',  32.0000,     396800.00,  '2025-01-10T08:00:00'),
    ( 8, 1, N'627-01',  N'Pavement Marking, Thermoplastic, White',  156000.0000, N'LF', 1.8500,     288600.00,  '2025-01-10T08:00:00'),
    ( 9, 1, N'627-02',  N'Pavement Marking, Thermoplastic, Yellow', 78000.0000, N'LF',  1.9500,     152100.00,  '2025-01-10T08:00:00'),
    (10, 1, N'630-01',  N'Temporary Traffic Control',                1.0000, N'LS',   425000.0000,  425000.00,  '2025-01-10T08:00:00'),

    -- Project 2: US-30 Bridge Replacement
    (11, 2, N'110-01',  N'Mobilization',                            1.0000, N'LS',   340000.0000,  340000.00,  '2023-04-15T10:00:00'),
    (12, 2, N'202-01',  N'Removal of Existing Bridge',              1.0000, N'LS',   285000.0000,  285000.00,  '2023-04-15T10:00:00'),
    (13, 2, N'501-01',  N'Structural Concrete, Class A',            1850.0000, N'CY',  625.0000,    1156250.00, '2023-04-15T10:00:00'),
    (14, 2, N'501-02',  N'Structural Concrete, Class S',            420.0000, N'CY',   750.0000,    315000.00,  '2023-04-15T10:00:00'),
    (15, 2, N'602-01',  N'Steel Reinforcing, Grade 60',             385000.0000, N'LB', 1.1500,     442750.00,  '2023-04-15T10:00:00'),
    (16, 2, N'602-02',  N'Epoxy-Coated Steel Reinforcing',          128000.0000, N'LB', 1.4500,     185600.00,  '2023-04-15T10:00:00'),
    (17, 2, N'507-01',  N'Prestressed Concrete Beam, Type IV',      14.0000, N'EA',   85000.0000,  1190000.00, '2023-04-15T10:00:00'),
    (18, 2, N'511-01',  N'Bridge Railing, Type T4',                 620.0000, N'LF',   195.0000,    120900.00,  '2023-04-15T10:00:00'),
    (19, 2, N'509-01',  N'Elastomeric Bearing Pads',                28.0000, N'EA',   2800.0000,    78400.00,   '2023-04-15T10:00:00'),

    -- Project 3: SR-9 Safety Improvement
    (20, 3, N'110-01',  N'Mobilization',                            1.0000, N'LS',   210000.0000,  210000.00,  '2025-05-20T09:30:00'),
    (21, 3, N'606-01',  N'Guardrail, W-Beam, Type 1',              18500.0000, N'LF',  31.5000,     582750.00,  '2025-05-20T09:30:00'),
    (22, 3, N'606-03',  N'Guardrail End Treatment, MASH TL-3',      42.0000, N'EA',   3200.0000,    134400.00,  '2025-05-20T09:30:00'),
    (23, 3, N'606-05',  N'Cable Barrier, High-Tension',             8400.0000, N'LF',  45.0000,     378000.00,  '2025-05-20T09:30:00'),
    (24, 3, N'620-01',  N'Traffic Signal, Complete Intersection',    3.0000, N'EA',   185000.0000,  555000.00,  '2025-05-20T09:30:00'),
    (25, 3, N'627-01',  N'Pavement Marking, Thermoplastic, White',  62000.0000, N'LF', 1.8500,      114700.00,  '2025-05-20T09:30:00'),
    (26, 3, N'614-01',  N'Rumble Strips, Shoulder',                 45000.0000, N'LF', 0.5500,      24750.00,   '2025-05-20T09:30:00'),

    -- Project 4: Downtown Multimodal Transit Hub
    (27, 4, N'110-01',  N'Mobilization',                            1.0000, N'LS',   1600000.0000, 1600000.00, '2025-11-01T13:00:00'),
    (28, 4, N'501-01',  N'Structural Concrete, Class A',            8500.0000, N'CY',  680.0000,    5780000.00, '2025-11-01T13:00:00'),
    (29, 4, N'602-01',  N'Steel Reinforcing, Grade 60',             1250000.0000, N'LB', 1.2000,    1500000.00, '2025-11-01T13:00:00'),
    (30, 4, N'550-01',  N'Structural Steel, ASTM A709 Gr 50',      480.0000, N'TON',  4500.0000,   2160000.00, '2025-11-01T13:00:00'),
    (31, 4, N'620-01',  N'Traffic Signal, Complete Intersection',    5.0000, N'EA',   195000.0000,  975000.00,  '2025-11-01T13:00:00'),
    (32, 4, N'640-01',  N'Lighting, LED, Complete Assembly',        85.0000, N'EA',   8500.0000,    722500.00,  '2025-11-01T13:00:00'),
    (33, 4, N'627-01',  N'Pavement Marking, Thermoplastic, White',  34000.0000, N'LF', 1.8500,      62900.00,   '2025-11-01T13:00:00'),

    -- Project 5: Route 119 Drainage Rehabilitation
    (34, 5, N'110-01',  N'Mobilization',                            1.0000, N'LS',   125000.0000,  125000.00,  '2024-07-10T07:45:00'),
    (35, 5, N'611-01',  N'Drainage Pipe, 18" RCP, Class III',       2800.0000, N'LF',  68.0000,     190400.00,  '2024-07-10T07:45:00'),
    (36, 5, N'611-02',  N'Drainage Pipe, 24" RCP, Class III',       1950.0000, N'LF',  92.0000,     179400.00,  '2024-07-10T07:45:00'),
    (37, 5, N'611-05',  N'Drainage Pipe, 36" RCP, Class IV',        650.0000, N'LF',   145.0000,    94250.00,   '2024-07-10T07:45:00'),
    (38, 5, N'612-01',  N'Drop Inlet, Type A',                      24.0000, N'EA',   4800.0000,    115200.00,  '2024-07-10T07:45:00'),
    (39, 5, N'612-03',  N'Junction Box, Precast',                   8.0000, N'EA',    6500.0000,    52000.00,   '2024-07-10T07:45:00'),
    (40, 5, N'613-01',  N'Flared End Section, 18"',                 18.0000, N'EA',   1250.0000,    22500.00,   '2024-07-10T07:45:00');

SET IDENTITY_INSERT [dbo].[ProjectItems] OFF;
GO

-- =============================================================================
-- FundingSources  (12 sources with realistic federal program identifiers)
-- =============================================================================
SET IDENTITY_INSERT [dbo].[FundingSources] ON;

INSERT INTO [dbo].[FundingSources]
    ([FundingSourceId], [ProjectId], [SourceName], [FundingIdentifier], [AllocatedAmount], [CreatedDate])
VALUES
    ( 1, 1, N'National Highway Performance Program (NHPP)',     N'NHPP-OH-2025-0142',   12950000.00, '2025-01-10T08:00:00'),
    ( 2, 1, N'State Transportation Fund',                       N'STF-OH-FY2025-0387',   3700000.00, '2025-01-10T08:00:00'),
    ( 3, 1, N'Local Match - Hamilton County',                    NULL,                    1850000.00, '2025-01-10T08:00:00'),

    ( 4, 2, N'FHWA Bridge Replacement Program',                N'FHWA-BR-IN-2023-0058', 4725000.00, '2023-04-15T10:00:00'),
    ( 5, 2, N'Surface Transportation Block Grant (STBG)',       N'STBG-IN-2023-0211',    1350000.00, '2023-04-15T10:00:00'),
    ( 6, 2, N'State Transportation Fund',                       N'STF-IN-FY2023-0194',    675000.00, '2023-04-15T10:00:00'),

    ( 7, 3, N'Highway Safety Improvement Program (HSIP)',       N'HSIP-WV-2025-0033',    3360000.00, '2025-05-20T09:30:00'),
    ( 8, 3, N'State Safety Fund',                               N'SSF-WV-FY2025-0076',    840000.00, '2025-05-20T09:30:00'),

    ( 9, 4, N'Federal Transit Administration (FTA) Grant',      N'FTA-KY-2025-0012',    22400000.00, '2025-11-01T13:00:00'),
    (10, 4, N'STBG - Urban',                                   N'STBG-URB-KY-2025-0091', 6400000.00, '2025-11-01T13:00:00'),
    (11, 4, N'City of Lexington Capital Fund',                   NULL,                    3200000.00, '2025-11-01T13:00:00'),

    (12, 5, N'Surface Transportation Block Grant (STBG)',       N'STBG-PA-2024-0178',    2100000.00, '2024-07-10T07:45:00');

SET IDENTITY_INSERT [dbo].[FundingSources] OFF;
GO

-- =============================================================================
-- ValidationIssues  (20 issues across all severity levels)
-- =============================================================================
SET IDENTITY_INSERT [dbo].[ValidationIssues] ON;

INSERT INTO [dbo].[ValidationIssues]
    ([ValidationIssueId], [ProjectId], [Severity], [Description], [AffectedField], [DetectedDate])
VALUES
    -- Project 1
    ( 1, 1, N'Warning', N'Total bid item amounts ($10,844,500) do not match project budget ($18,500,000). Remaining funds are unallocated to line items.', N'TotalBudget',  '2025-09-22T14:35:00'),
    ( 2, 1, N'Info',    N'Milling quantity (185,000 SY) exceeds typical range for corridor length. Verify full-width milling is intended.',              N'Quantity',     '2025-09-10T09:00:00'),
    ( 3, 1, N'Error',   N'Project end date is not set. Active projects must have a scheduled completion date.',                                          N'EndDate',      '2025-09-22T14:35:00'),
    ( 4, 1, N'Warning', N'PG 76-22 binder grade may be over-specified for this climate zone. Verify against LTPPBind recommendations.',                  N'ItemNumber',   '2025-08-15T11:20:00'),

    -- Project 2
    ( 5, 2, N'Info',    N'Project completed within original budget. Final cost was $4,113,900 under estimate.',                                          NULL,            '2025-01-20T16:00:00'),
    ( 6, 2, N'Warning', N'Bridge railing type T4 has been superseded by MASH-compliant TL-4 in current standards.',                                      N'Description',  '2024-11-05T08:30:00'),
    ( 7, 2, N'Info',    N'Prestressed beam type IV is adequate for the 85-ft span. No design concern.',                                                  N'ItemNumber',   '2023-08-20T14:00:00'),

    -- Project 3
    ( 8, 3, N'Error',   N'Project status is OnHold but no hold reason has been documented. State policy requires a justification memo.',                  N'Status',       '2025-08-10T11:15:00'),
    ( 9, 3, N'Warning', N'HSIP funding requires crash data analysis report (Form HSIP-3). Report has not been uploaded.',                                 NULL,            '2025-07-28T10:00:00'),
    (10, 3, N'Error',   N'Cable barrier item 606-05 unit price ($45.00/LF) is below the current state minimum bid threshold of $48.00/LF.',              N'UnitPrice',    '2025-08-01T16:45:00'),
    (11, 3, N'Info',    N'Rumble strip item 614-01 quantity aligns with corridor length and shoulder width.',                                              N'Quantity',     '2025-06-15T09:00:00'),
    (12, 3, N'Warning', N'Three traffic signal installations may require utility relocation coordination. Verify with local utility providers.',          N'Description',  '2025-07-02T13:30:00'),

    -- Project 4
    (13, 4, N'Error',   N'Project is in Draft status but has allocated funding totaling $32,000,000. Funding commitments require at minimum Active status.', N'Status',    '2025-12-18T09:45:00'),
    (14, 4, N'Warning', N'Structural steel quantity (480 TON) should be reconciled with final structural drawings. Current estimate is preliminary.',      N'Quantity',     '2025-12-10T14:00:00'),
    (15, 4, N'Error',   N'FTA grant FTA-KY-2025-0012 requires an approved Environmental Assessment (EA). Document not found in project records.',         N'FundingIdentifier', '2025-12-15T08:00:00'),
    (16, 4, N'Info',    N'LED lighting assemblies (85 EA) include 20-year maintenance warranty per manufacturer specification.',                           N'Description',  '2025-11-20T10:30:00'),
    (17, 4, N'Warning', N'Project start date is less than 30 days away but no Notice to Proceed has been issued.',                                        N'StartDate',    '2025-12-18T09:45:00'),

    -- Project 5
    (18, 5, N'Info',    N'Project was cancelled due to environmental permitting delays at wetland crossings near MP 6.4.',                                N'Status',       '2025-03-01T10:00:00'),
    (19, 5, N'Error',   N'STBG funds ($2,100,000) were obligated but project was cancelled. De-obligation request must be submitted to FHWA.',            N'AllocatedAmount', '2025-03-01T10:00:00'),
    (20, 5, N'Warning', N'Drop inlet Type A (item 612-01) was specified but site conditions may require Type C for higher flow capacity.',                N'Description',  '2024-12-05T15:20:00');

SET IDENTITY_INSERT [dbo].[ValidationIssues] OFF;
GO
