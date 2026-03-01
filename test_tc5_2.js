const axios = require('axios');

async function test_tc52_asset_clearance() {
    console.log('--- Starting TC 5.2: Verify Asset Clearance Warning Data ---');
    const API_URL = 'http://localhost:5043/api/v1/Dashboard/overview';

    try {
        console.log(`Fetching dashboard overview from: ${API_URL}`);
        const response = await axios.get(API_URL);

        if (response.status === 200) {
            console.log('✅ API Request Successful');
            const data = response.data.data;

            // Check if pendingAssetClearances property exists
            if (data.hasOwnProperty('pendingAssetClearances')) {
                console.log('✅ Found "pendingAssetClearances" property in API response.');
                console.log(`Count: ${data.pendingAssetClearances.length}`);

                // If there's data, verify structure
                if (data.pendingAssetClearances.length > 0) {
                    const sample = data.pendingAssetClearances[0];
                    const requiredFields = ['employeeId', 'employeeName', 'assetName', 'assetCode', 'departureDate'];

                    let allFieldsPresent = true;
                    requiredFields.forEach(field => {
                        if (!sample.hasOwnProperty(field)) {
                            console.error(`❌ Missing field: ${field}`);
                            allFieldsPresent = false;
                        }
                    });

                    if (allFieldsPresent) {
                        console.log('✅ Data structure is correct.');
                    } else {
                        throw new Error('Data structure mismatch');
                    }
                } else {
                    console.log('ℹ️ No pending asset clearances at the moment (correct for current state).');
                }
            } else {
                console.error('❌ Property "pendingAssetClearances" MISSING in response');
                console.log('Full Response Keys:', Object.keys(data));
                throw new Error('API Model Mismatch');
            }
        } else {
            console.error(`❌ Unexpected status code: ${response.status}`);
            throw new Error('API Error');
        }

        console.log('\n--- TC 5.2 VERIFIED SUCCESSFULLY ---');
    } catch (error) {
        console.error('❌ TC 5.2 FAILED:', error.message);
        if (error.response) {
            console.error('Response Data:', JSON.stringify(error.response.data, null, 2));
        }
        process.exit(1);
    }
}

test_tc52_asset_clearance();
