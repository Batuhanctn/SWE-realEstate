import axios from 'axios';
import { Property } from '../types/property';

const API_URL = 'http://localhost:5150/api';

/**
 * Adds a property to the user's favorites list
 * @param {string} propertyId - The ID of the property to add to favorites
 * @returns {Promise<any>} A promise that resolves when the property is added to favorites
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const addToFavorites = async (propertyId: string) => {
  const token = localStorage.getItem('token');
  console.log('Adding to favorites:', { propertyId });
  const response = await axios.post(
    `${API_URL}/favorite`,
    { propertyId },
    {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
    }
  );
  return response.data;
};

/**
 * Removes a property from the user's favorites list
 * @param {string} propertyId - The ID of the property to remove from favorites
 * @returns {Promise<any>} A promise that resolves when the property is removed from favorites
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const removeFromFavorites = async (propertyId: string) => {
  const token = localStorage.getItem('token');
  console.log('Removing from favorites:', { propertyId });
  const response = await axios.delete(`${API_URL}/favorite/${propertyId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return response.data;
};

/**
 * Retrieves all properties in the user's favorites list
 * @returns {Promise<Property[]>} A promise that resolves to an array of favorite Property objects
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const getFavorites = async (): Promise<Property[]> => {
  const token = localStorage.getItem('token');
  console.log('Getting favorites');
  const response = await axios.get(`${API_URL}/favorite`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return response.data;
};

/**
 * Checks if a property is in the user's favorites list
 * @param {string} propertyId - The ID of the property to check
 * @returns {Promise<boolean>} A promise that resolves to true if the property is in favorites, false otherwise
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const checkIsFavorite = async (propertyId: string): Promise<boolean> => {
  const token = localStorage.getItem('token');
  console.log('Checking if favorite:', { propertyId });
  try {
    const response = await axios.get(`${API_URL}/favorite/${propertyId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error('Error checking favorite:', error);
    return false;
  }
};
