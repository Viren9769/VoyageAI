# UPDATE TRIP WORKFLOW GUIDE

## The Problem (OLD WAY - Not Recommended)
```
❌ Don't do this:
   1. User clicks "Edit"
   2. User has to MANUALLY RE-ENTER all trip details again
   3. Send PUT request with ALL fields
   4. Very tedious and error-prone
```

## The Solution (NEW WAY - Recommended)
```
✅ Do this instead:
   1. GET /api/trips/{tripId}     ← Fetch current data
   2. Pre-fill form with data      ← Show user what's currently there
   3. User edits only changed fields
   4. PUT /api/trips/{tripId}     ← Send ONLY changed fields
```

---

## STEP-BY-STEP WORKFLOW

### Step 1: User Clicks "Edit Trip"
```
Frontend Action:
  → Calls: GET /api/trips/550e8400-e29b-41d4-a716-446655440000
  → Authorization: Bearer {jwt_token}
```

### Step 2: API Returns Current Trip Data
```json
{
  "tripId": "550e8400-e29b-41d4-a716-446655440000",
  "tripName": "Summer Europe Adventure 2025",
  "destinationCountry": "France",
  "destinationCity": "Paris",
  "startDate": "2025-06-01T00:00:00Z",
  "endDate": "2025-06-15T00:00:00Z",
  "budget": 5000,
  "currency": "USD",
  "travelStyle": "Luxury",
  "description": "Amazing summer trip to Paris",
  "coverImageUrl": "https://example.com/paris.jpg",
  "status": "Planning",
  "createdAt": "2025-01-06T14:30:00Z",
  "updatedAt": "2025-01-06T14:30:00Z",
  "durationDays": 15
}
```

### Step 3: Frontend Pre-fills the Edit Form
```
Form Fields Populated with Current Values:
┌─────────────────────────────────────────────┐
│ Edit Trip                                    │
├─────────────────────────────────────────────┤
│ Trip Name:        Summer Europe Adv...      │  ← Pre-filled
│ Country:          France                    │  ← Pre-filled
│ City:             Paris                     │  ← Pre-filled
│ Start Date:       2025-06-01                │  ← Pre-filled
│ End Date:         2025-06-15                │  ← Pre-filled
│ Budget:           5000                      │  ← Pre-filled
│ Currency:         USD                       │  ← Pre-filled
│ Travel Style:     Luxury                    │  ← Pre-filled
│ Description:      Amazing summer...         │  ← Pre-filled
│ Status:           Planning                  │  ← Pre-filled
│                                              │
│  [Save Changes]  [Cancel]                   │
└─────────────────────────────────────────────┘
```

### Step 4: User Edits Only What They Want to Change
```
User Changes:
  1. Increases Budget from 5000 → 6000
  2. Changes Status from "Planning" → "Confirmed"

Everything else stays as-is!
```

### Step 5: Frontend Sends Only Changed Fields
```
PUT /api/trips/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "budget": 6000,
  "status": "Confirmed"
}
```

**KEY POINT:** Only 2 fields sent! Not the entire trip data!

### Step 6: API Merges Changes and Returns Full Updated Trip
```json
{
  "tripId": "550e8400-e29b-41d4-a716-446655440000",
  "tripName": "Summer Europe Adventure 2025",      ← Unchanged ✓
  "destinationCountry": "France",                  ← Unchanged ✓
  "destinationCity": "Paris",                      ← Unchanged ✓
  "startDate": "2025-06-01T00:00:00Z",             ← Unchanged ✓
  "endDate": "2025-06-15T00:00:00Z",               ← Unchanged ✓
  "budget": 6000,                                  ← UPDATED ✓
  "currency": "USD",                               ← Unchanged ✓
  "travelStyle": "Luxury",                         ← Unchanged ✓
  "description": "Amazing summer trip to Paris",   ← Unchanged ✓
  "coverImageUrl": "https://example.com/paris.jpg",← Unchanged ✓
  "status": "Confirmed",                           ← UPDATED ✓
  "createdAt": "2025-01-06T14:30:00Z",
  "updatedAt": "2025-01-06T15:45:00Z",             ← Server updated
  "durationDays": 15
}
```

---

## API BEHAVIOR EXPLAINED

### ✅ What Happens With Your Partial Update Request:

```
Backend Logic in TripService.UpdateTripAsync():

1. Fetch current trip from database
   current = { budget: 5000, status: "Planning", ... }

2. Check if field is in request
   if (request.budget has value) → update trip.budget = 6000
   if (request.status has value) → update trip.status = "Confirmed"
   if (request.tripName is null) → KEEP current trip.tripName
   if (request.destinationCountry is null) → KEEP current destination

3. Save to database
   UPDATE Trips SET budget=6000, status='Confirmed', UpdatedAt=NOW()
   WHERE TripId = '550e8400-...'

4. Return full trip object
```

### Key Features:

| Feature | Description |
|---------|-------------|
| **Partial Update** | Send only fields you want to change |
| **Auto-Merge** | Backend automatically keeps unchanged fields |
| **Form Pre-fill** | GET first, then PUT with changes |
| **Error Prevention** | No accidental overwrites |
| **Network Efficient** | Less data to transfer |
| **User-Friendly** | Works like normal form editing |

---

## REAL-WORLD EXAMPLE: CHANGING ONLY THE BUDGET

### Scenario:
User realized they have more budget for the trip. They want to change it from $5000 to $6000.

### Old Way (❌ DON'T):
```
PUT /api/trips/{id}
{
	"tripName": "Summer Europe Adventure 2025",        ← Re-enter (tedious)
	"destinationCountry": "France",                    ← Re-enter (tedious)
	"destinationCity": "Paris",                        ← Re-enter (tedious)
	"startDate": "2025-06-01T00:00:00Z",               ← Re-enter (tedious)
	"endDate": "2025-06-15T00:00:00Z",                 ← Re-enter (tedious)
	"budget": 6000,                                    ← Only this changed!
	"currency": "USD",                                 ← Re-enter (tedious)
	"travelStyle": "Luxury",                           ← Re-enter (tedious)
	"description": "Amazing summer trip to Paris",     ← Re-enter (tedious)
	"coverImageUrl": "https://example.com/paris.jpg", ← Re-enter (tedious)
	"status": "Planning"                               ← Re-enter (tedious)
}
```
⚠️ Problems: Tedious, error-prone, lots of data, user has to remember all values

### New Way (✅ DO):
```
1. GET /api/trips/{id}
   ↓ (Frontend loads current values)

2. Form pre-filled with all current values
   ↓ (User sees everything, only changes Budget field)

3. PUT /api/trips/{id}
{
	"budget": 6000
}
```
✅ Benefits: Simple, clean, user-friendly, error-proof

---

## CODE EXAMPLE: React/JavaScript Frontend

```javascript
import React, { useState, useEffect } from 'react';

function EditTripForm({ tripId, token }) {
  const [formData, setFormData] = useState({});
  const [originalData, setOriginalData] = useState({});
  const [isLoading, setIsLoading] = useState(true);

  // Step 1: Fetch current trip data on component mount
  useEffect(() => {
	const fetchTrip = async () => {
	  const response = await fetch(`/api/trips/${tripId}`, {
		headers: { 'Authorization': `Bearer ${token}` }
	  });
	  const trip = await response.json();
	  setFormData(trip);
	  setOriginalData(trip);  // Keep original for comparison
	  setIsLoading(false);
	};
	fetchTrip();
  }, [tripId, token]);

  // Step 2: Handle form input changes
  const handleChange = (e) => {
	const { name, value } = e.target;
	setFormData(prev => ({
	  ...prev,
	  [name]: value
	}));
  };

  // Step 3: Submit only changed fields
  const handleSubmit = async (e) => {
	e.preventDefault();

	// Calculate which fields changed
	const changedFields = {};
	Object.keys(formData).forEach(key => {
	  if (formData[key] !== originalData[key]) {
		changedFields[key] = formData[key];
	  }
	});

	// If nothing changed, show message
	if (Object.keys(changedFields).length === 0) {
	  alert('No changes made');
	  return;
	}

	// Send ONLY changed fields
	const response = await fetch(`/api/trips/${tripId}`, {
	  method: 'PUT',
	  headers: {
		'Authorization': `Bearer ${token}`,
		'Content-Type': 'application/json'
	  },
	  body: JSON.stringify(changedFields)  // ← Only changed fields!
	});

	if (response.ok) {
	  const updatedTrip = await response.json();
	  alert('Trip updated successfully!');
	  setOriginalData(updatedTrip);  // Reset for next edit
	  setFormData(updatedTrip);
	}
  };

  if (isLoading) return <div>Loading...</div>;

  return (
	<form onSubmit={handleSubmit}>
	  <h2>Edit Trip</h2>

	  {/* Pre-filled with current data */}
	  <input
		name="tripName"
		value={formData.tripName || ''}
		onChange={handleChange}
		placeholder="Trip Name"
	  />

	  <input
		name="destinationCountry"
		value={formData.destinationCountry || ''}
		onChange={handleChange}
		placeholder="Country"
	  />

	  <input
		name="budget"
		type="number"
		value={formData.budget || 0}
		onChange={handleChange}
		placeholder="Budget"
	  />

	  {/* Only days "Confirmed" shows in the request if user changes it */}
	  <select
		name="status"
		value={formData.status || ''}
		onChange={handleChange}
	  >
		<option value="Planning">Planning</option>
		<option value="Confirmed">Confirmed</option>
		<option value="Completed">Completed</option>
		<option value="Cancelled">Cancelled</option>
	  </select>

	  <button type="submit">Save Changes</button>
	</form>
  );
}
```

---

## SUMMARY

| Aspect | Details |
|--------|---------|
| **Endpoint** | `PUT /api/trips/{tripId}` |
| **Pre-requisite** | Call `GET /api/trips/{tripId}` first |
| **Send** | Only changed fields (partial update) |
| **Backend** | Auto-merges changes with existing data |
| **Receive** | Full updated trip object |
| **Benefit** | User-friendly, efficient, error-proof |

**Your API is already built to support this workflow perfectly! 🎉**
