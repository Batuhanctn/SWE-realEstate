
import { Hero } from '../components/Hero';
import { PropertyList } from '../components/PropertyList';

export const Home = () => {
  return (
    <div>
      <Hero />
      <div className="-mt-32 relative z-20">
        <PropertyList />
      </div>
    </div>
  );
};
