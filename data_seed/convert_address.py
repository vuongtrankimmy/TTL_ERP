import json
import os

input_path = 'full_address.json'
with open(input_path, 'r', encoding='utf-8') as f:
    raw_data = json.load(f)

country_id = "65fc5b5b0000000000000000"

provinces = []
districts = []
wards = []
provinces_translate = []

def pad(val, length):
    return str(val).zfill(length)

for p_idx, p in enumerate(raw_data):
    p_code = pad(p['code'], 2)
    # Province ID: 65fc5b5b00000000000001##
    p_id = f"65fc5b5b00000000000001{p_code}"
    
    provinces.append({
        "_id": { "$oid": p_id },
        "CountryId": { "$oid": country_id },
        "Code": p_code,
        "Name": p['name'],
        "Type": p['division_type'],
        "Order": p_idx + 1
    })

    provinces_translate.append({
        "_id": { "$oid": f"65fc5b5b00000000000011{p_code}" },
        "ProvinceId": { "$oid": p_id },
        "LanguageCode": "vi-VN",
        "Name": p['name']
    })

    for d in p.get('districts', []):
        d_code = pad(d['code'], 3)
        # District ID: 65fc5b5b000000000000####
        d_id = f"65fc5b5b000000000000{pad(d['code'], 4)}"
        
        districts.append({
            "_id": { "$oid": d_id },
            "ProvinceId": { "$oid": p_id },
            "Code": d_code,
            "Name": d['name'],
            "Type": d['division_type']
        })

        for w in d.get('wards', []):
            w_code = pad(w['code'], 5)
            # Ward ID: 65fc5b5b000000000#######
            w_id = f"65fc5b5b000000000{pad(w['code'], 7)}"
            
            wards.append({
                "_id": { "$oid": w_id },
                "DistrictId": { "$oid": d_id },
                "ProvinceId": { "$oid": p_id },
                "Code": w_code,
                "Name": w['name'],
                "Type": w['division_type']
            })

with open('provinces.json', 'w', encoding='utf-8') as f:
    json.dump(provinces, f, ensure_ascii=False, indent=2)
with open('districts.json', 'w', encoding='utf-8') as f:
    json.dump(districts, f, ensure_ascii=False, indent=2)
with open('wards.json', 'w', encoding='utf-8') as f:
    json.dump(wards, f, ensure_ascii=False, indent=2)
with open('provinces_translate.json', 'w', encoding='utf-8') as f:
    json.dump(provinces_translate, f, ensure_ascii=False, indent=2)

print(f"Generated: {len(provinces)} provinces, {len(districts)} districts, {len(wards)} wards.")
