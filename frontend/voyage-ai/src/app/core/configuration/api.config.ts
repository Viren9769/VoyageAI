import { environment } from '../../../environments/environment';

export const ApiConfig = {

  baseUrl: environment.apiUrl,

  auth: {
    register: '/Auth/register',
    login: '/Auth/login',
    refreshToken: '/Auth/refresh-token'
  },

  trips: {
    base: '/Trips'
  },

  travelers: {
    base: '/Trips'
  },

  itinerary: {
    base: '/Trips'
  },

  activities: {
    base: '/Trips'
  }

};