const fs = require('fs');
const path = require('path');

const inputPath = path.join(__dirname, 'full_address.json');
const rawData = JSON.parse(fs.readFileSync(inputPath, 'utf8'));

const countryId = "65fc5b5b0000000000000000";

const provinces = [];
const districts = [];
const wards = [];
const provincesTranslate = [];

function pad(str, length) {
    return str.toString().padStart(length, '0');
}

rawData.forEach((p, pIdx) => {
    const pCode = pad(p.code, 2);
    // Province ID: 65fc5b5b00000000000001##
    const pId = `65fc5b5b00000000000001${pCode}`;
    
    provinces.push({
        "_id": { "$oid": pId },
        "CountryId": { "$oid": countryId },
        "Code": pCode,
        "Name": p.name,
        "Type": p.division_type,
        "Order": pIdx + 1
    });

    provincesTranslate.push({
        "_id": { "$oid": `65fc5b5b00000000000011${pCode}` },
        "ProvinceId": { "$oid": pId },
        "LanguageCode": "vi-VN",
        "Name": p.name
    });

    p.districts.forEach((d) => {
        const dCode = pad(d.code, 3);
        // District ID: 65fc5b5b000000000000####
        const dId = `65fc5b5b000000000000${pad(d.code, 4)}`;
        
        districts.push({
            "_id": { "$oid": dId },
            "ProvinceId": { "$oid": pId },
            "Code": dCode,
            "Name": d.name,
            "Type": d.division_type
        });

        d.wards.forEach((w) => {
            const wCode = pad(w.code, 5);
            // Ward ID: 65fc5b5b000000000#######
            const wId = `65fc5b5b000000000${pad(w.code, 7)}`;
            
            wards.push({
                "_id": { "$oid": wId },
                "DistrictId": { "$oid": dId },
                "ProvinceId": { "$oid": pId },
                "Code": wCode,
                "Name": w.name,
                "Type": w.division_type
            });
        });
    });
});

fs.writeFileSync(path.join(__dirname, 'provinces.json'), JSON.stringify(provinces, null, 2));
fs.writeFileSync(path.join(__dirname, 'districts.json'), JSON.stringify(districts, null, 2));
fs.writeFileSync(path.join(__dirname, 'wards.json'), JSON.stringify(wards, null, 2));
fs.writeFileSync(path.join(__dirname, 'provinces_translate.json'), JSON.stringify(provincesTranslate, null, 2));

console.log(`Generated: ${provinces.length} provinces, ${districts.length} districts, ${wards.length} wards.`);
