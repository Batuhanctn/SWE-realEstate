import axios from 'axios';
import { Property } from '../types/property';

const API_URL = 'http://localhost:5150/api';

/**
 * Retrieves all properties from the API
 * @returns {Promise<Property[]>} A promise that resolves to an array of Property objects
 * @throws {Error} If the API request fails
 */
export const getProperties = async (): Promise<Property[]> => {
  try {
    const response = await axios.get(`${API_URL}/properties`);
    console.log('Properties retrieved successfully:', response.data);
    return response.data;
  } catch (error: any) {
    console.error('Error details:', {
      status: error.response?.status,
      data: error.response?.data,
      headers: error.response?.headers,
    });
    throw error;
  }
};

/**
 * Retrieves a specific property by ID
 * @param {string} Id - The ID of the property to retrieve
 * @returns {Promise<Property>} A promise that resolves to a Property object
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const getProperty = async (Id: string): Promise<Property> => {
  const token = localStorage.getItem('token');
  try {
    const response = await axios.get(`${API_URL}/properties/${Id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    console.log('Property retrieved successfully:', response.data);
    return response.data;
  } catch (error: any) {
    console.error('Error details:', {
      status: error.response?.status,
      data: error.response?.data,
      headers: error.response?.headers,
    });
    throw error;
  }
};

/**
 * Retrieves all properties belonging to the authenticated user
 * @returns {Promise<Property[]>} A promise that resolves to an array of Property objects
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const getUserProperties = async (): Promise<Property[]> => {
  const token = localStorage.getItem('token');
  try {
    const response = await axios.get(`${API_URL}/properties/user`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    console.log('User properties retrieved successfully:', response.data);
    return response.data;
  } catch (error: any) {
    console.error('Error details:', {
      status: error.response?.status,
      data: error.response?.data,
      headers: error.response?.headers,
    });
    throw error;
  }
};

/**
 * Searches for properties based on a query string
 * @param {string} query - The search query
 * @returns {Promise<Property[]>} A promise that resolves to an array of matching Property objects
 * @throws {Error} If the API request fails
 */
export const searchProperties = async (query: string): Promise<Property[]> => {
  try {
    console.log('Searching properties with query:', query);
    const response = await axios.get(`${API_URL}/properties/search`, {
      params: { query },
    });
    console.log('Properties retrieved successfully:', response.data);
    return response.data;
  } catch (error: any) {
    console.error('Error details:', {
      status: error.response?.status,
      data: error.response?.data,
      headers: error.response?.headers,
    });
    throw error;
  }
};

/**
 * Creates a new property listing
 * @param {any} propertyData - The property data to create
 * @returns {Promise<Property>} A promise that resolves to the created Property object
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const createProperty = async (propertyData: any): Promise<Property> => {
  const token = localStorage.getItem('token');
  const userDataStr = localStorage.getItem('user');
  
  if (!userDataStr) {
    throw new Error('User not authenticated');
  }

  const userData = JSON.parse(userDataStr);
  
  // Ensure we have a user ID from the stored user data
  if (!userData.id) {
    console.error('User data:', userData);
    throw new Error('User ID not found in stored data');
  }
  
  try {
    // First, create the property without images
    const { images, ...propertyDetails } = propertyData;
    
    // Convert the data to match the API's expectations
    const requestData = {
      ...propertyDetails,
      // Ensure required fields are present
      location: propertyDetails.location || propertyDetails.address || '',
      address: propertyDetails.address || propertyDetails.location || '',
      city: propertyDetails.city || propertyData.location || '',
      state: propertyDetails.state || 'Default State',
      zipCode: propertyDetails.zipCode || '00000',
      type: propertyDetails.propertyType || 'Apartment',
      // Add any other required defaults
      imageUrl: '',
      imageUrls: [],
      images: [],
      features: propertyDetails.features || [],
      isForRent: propertyDetails.isForRent || false,
      size: propertyDetails.squareMeters || 0,
      userId: userData.id // Using the lowercase 'id' from user data
    };
    
    console.log('Submitting property data:', requestData);
    
    const response = await axios.post(`${API_URL}/properties`, requestData, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      },
    });

    console.log('Property created successfully:', response.data);

    // If there are images, upload them separately
    if (images && images.length > 0) {
      const propertyId = response.data.Id;
      const formData = new FormData();
      images.forEach((image: File) => {
        formData.append('images', image);
      });

      console.log('Uploading images for property:', propertyId);
      
      const imageResponse = await axios.post(`${API_URL}/properties/${propertyId}/images`, formData, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'multipart/form-data',
        },
      });
      
      console.log('Images uploaded successfully:', imageResponse.data);
    }

    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      console.error('Error creating property:');
      console.error('Status:', error.response?.status);
      console.error('Status Text:', error.response?.statusText);
      console.error('Error Data:', error.response?.data);
      console.error('Request Data:', error.config?.data);
      console.error('Headers:', error.config?.headers);
      
      if (error.response?.data?.errors) {
        console.error('Validation Errors:', error.response.data.errors);
      }
    }
    throw error;
  }
};

/**
 * Updates an existing property
 * @param {string} Id - The ID of the property to update
 * @param {Partial<Property>} propertyData - The property data to update
 * @returns {Promise<Property>} A promise that resolves to the updated Property object
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const updateProperty = async (Id: string, propertyData: Partial<Property>): Promise<Property> => {
  const token = localStorage.getItem('token');
  try {
    console.log('Updating property with Id:', Id, 'and data:', propertyData);
    const response = await axios.put(`${API_URL}/properties/${Id}`, propertyData, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });
    console.log('Property updated successfully:', response.data);
    return response.data;
  } catch (error: any) {
    console.error('Error details:', {
      status: error.response?.status,
      data: error.response?.data,
      headers: error.response?.headers,
    });
    throw error;
  }
};

/**
 * Deletes a property
 * @param {string} Id - The ID of the property to delete
 * @returns {Promise<void>} A promise that resolves when the property is deleted
 * @throws {Error} If the API request fails or authentication is invalid
 */
export const deleteProperty = async (Id: string): Promise<void> => {
  const token = localStorage.getItem('token');
  try {
    console.log('Deleting property with Id:', Id);
    await axios.delete(`${API_URL}/properties/${Id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    console.log('Property deleted successfully');
  } catch (error: any) {
    console.error('Error details:', {
      status: error.response?.status,
      data: error.response?.data,
      headers: error.response?.headers,
    });
    throw error;
  }
};
